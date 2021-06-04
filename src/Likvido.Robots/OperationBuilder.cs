using System;
using System.Threading.Tasks;
using Likvido.ApplicationInsights.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Robots
{
    public class OperationBuilder : IOperationBuilder, IOperationSyncBuilder, IOperationAsyncBuilder
    {
        public OperationBuilder(string role, string name)
        {
            OperationDetails = new OperationDetails(role, name);
        }

        public OperationDetails OperationDetails { get; }

        void IOperationSyncBuilder.Run()
        {
            using var operation = ((IOperationSyncBuilder)this).Build();
            operation.Run();
        }

        Operation IOperationSyncBuilder.Build()
        {
            OperationDetails.AsyncFunc = sp => 
            {
                OperationDetails.Func!(sp);
                return Task.CompletedTask;
            };

            if (OperationDetails.PostExecute != null)
            {
                OperationDetails.AsyncPostExecute = () =>
                {
                    OperationDetails.PostExecute();
                    return Task.CompletedTask;
                };
            }

            var (func, _, dispose) = Build();
            return new Operation(() => func().GetAwaiter().GetResult(), dispose);
        }

        async Task IOperationAsyncBuilder.Run()
        {
            await using var operation = ((IOperationAsyncBuilder)this).Build();
            await operation.Run().ConfigureAwait(false);
        }

        AsyncOperation IOperationAsyncBuilder.Build()
        {
            var (func, asyncDispose, _) = Build();
            return new AsyncOperation(func, asyncDispose);
        }

        private (Func<Task> Func, Func<ValueTask> AsyncDispose, Action Dispose) Build()
        {
            var serviceProvider = OperationDetails.BuildServiceProvider();
            return (async () =>
                {
                    using var scope = serviceProvider.CreateScope();
                    var services = scope.ServiceProvider;
                    var telemetryClient = services.GetRequiredService<TelemetryClient>();
                    Func<Task> func = async () =>
                    {
                        try
                        {
                            await OperationDetails.AsyncFunc!(services).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            OperationDetails.ReportError?
                                .Invoke(services, $"Job run failed. Robot - {OperationDetails.Role}, operation - {OperationDetails.Name}", e);
                            throw;
                        }
                    };
                    var requestOptions = new ExecuteAsRequestAsyncOptions(OperationDetails.Name, func)
                    {
                        FlushWait = OperationDetails.FlushWait,
                        Configure = OperationDetails.ConfigureRequestTelemetry,
                        PostExecute = OperationDetails.AsyncPostExecute
                    };

                    await telemetryClient.ExecuteAsRequestAsync(requestOptions).ConfigureAwait(false);
                },
                serviceProvider.DisposeAsync,
                serviceProvider.Dispose);
        }
    }
}
