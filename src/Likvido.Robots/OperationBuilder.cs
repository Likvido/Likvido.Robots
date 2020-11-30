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
            ((IOperationSyncBuilder)this).Build().Invoke();
        }

        Action IOperationSyncBuilder.Build()
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

             var func = ((IOperationAsyncBuilder)this).Build();
            return () => func().GetAwaiter().GetResult();
        }

        async Task IOperationAsyncBuilder.Run()
        {
            await ((IOperationAsyncBuilder)this).Build().Invoke().ConfigureAwait(false);
        }

        Func<Task> IOperationAsyncBuilder.Build()
        {
            var serviceProvider = OperationDetails.BuildServiceProvider();

            return async () =>
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
                        OperationDetails.ReportError?.Invoke($"Job run failed. Robot - {OperationDetails.Role}, operation - {OperationDetails.Name}", e);
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
            };
        }
    }
}
