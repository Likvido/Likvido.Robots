using System;
using System.Threading.Tasks;
using Likvido.ApplicationInsights.Telemetry;
using Likvido.Configurations;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Robots
{
    public class OperationDetails
    {
        private ServiceProvider? _serviceProvider;

        public OperationDetails(string role, string name)
        {
            Name = name;
            Role = role;
        }

        public string Role { get; }
        public string Name { get; }
        public Action<IServiceProvider>? Func { get; set; }
        public Action? PostExecute { get; set; }

        public Func<IServiceProvider, Task>? AsyncFunc { get; set; }
        public Func<Task>? AsyncPostExecute { get; set; }
        public Action<IConfiguration, IServiceCollection>? ConfigureServices { get; set; }
        public Action<ServiceProvider>? OnServiceProviderBuild { get; set; }
        public ReportError? ReportError { get; set; }
        public int? FlushWait { get; set; } = 15;
        public Action<IOperationHolder<RequestTelemetry>>? ConfigureRequestTelemetry { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
        public string[]? Args { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        public ServiceProvider BuildServiceProvider()
        {
            if (_serviceProvider != null)
            {
                return _serviceProvider;
            }
            var appConfiguration = ConsoleAppConfiguration.Build(Args);

            var services = new ServiceCollection()
                .AddSingleton(appConfiguration)
                .AddSingleton<ITelemetryInitializer>(new ServiceNameInitializer(Role))
                .AddSingleton<ITelemetryInitializer>(new AvoidRequestSamplingTelemetryInitializer(Name))
                .AddApplicationInsightsTelemetryWorkerService(appConfiguration)
                .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

            ConfigureServices?.Invoke(appConfiguration, services);

            _serviceProvider = services.BuildServiceProvider();

            OnServiceProviderBuild?.Invoke(_serviceProvider);
            return _serviceProvider;
        }
    }
}
