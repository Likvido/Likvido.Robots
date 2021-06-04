using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Likvido.Robots
{
    public static class RobotOperationBuilderExtensions
    {
        /// <summary>
        /// Defines a callback to add any app specific services
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        public static T SetConfigureServices<T>(this T builder, Action<IConfiguration, IServiceCollection> configureServices)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.ConfigureServices = configureServices;
            return builder;
        }

        /// <summary>
        /// Once a service provider is built this callback will be called
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="onServiceProviderBuild"></param>
        /// <returns></returns>
        public static T SetOnServiceProviderBuild<T>(this T builder, Action<IServiceProvider> onServiceProviderBuild)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.OnServiceProviderBuild = onServiceProviderBuild;
            return builder;
        }

        /// <summary>
        /// Specifies callback to be called when an error occurs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="reportError"></param>
        /// <returns></returns>
        public static T SetReportError<T>(this T builder, Action<string, Exception> reportError)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.ReportError = (_, message, ex) => reportError?.Invoke(message, ex);
            return builder;
        }

        /// <summary>
        /// Specifies callback to be called when an error occurs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="reportError"></param>
        /// <returns></returns>
        public static T SetReportError<T>(this T builder, ReportError reportError)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.ReportError = reportError;
            return builder;
        }

        /// <summary>
        /// Defines timeout to send all metrics
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="flushWait"></param>
        /// <returns></returns>
        public static T SetFlushWait<T>(this T builder, int? flushWait)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.FlushWait = flushWait;
            return builder;
        }

        /// <summary>
        /// A callback to specify additional properties on RequestTelemetry
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configureRequestTelemetry"></param>
        /// <returns></returns>
        public static T SetConfigureRequestTelemetry<T>(this T builder, Action<IOperationHolder<RequestTelemetry>>? configureRequestTelemetry)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.ConfigureRequestTelemetry = configureRequestTelemetry;
            return builder;
        }

        /// <summary>
        /// Specifies args from command line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T SetArgs<T>(this T builder, string[] args)
            where T : IOperationBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.Args = args;
            return builder;
        }

        /// <summary>
        /// Set a sync action to be executed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IOperationSyncBuilder SetFunc(this IOperationBuilder builder, Action<IServiceProvider> func)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            builder.OperationDetails.Func = func;
            return (IOperationSyncBuilder)builder;
        }

        /// <summary>
        /// Set an async action to be executed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IOperationAsyncBuilder SetFunc(this IOperationBuilder builder, Func<IServiceProvider, Task> func)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            builder.OperationDetails.AsyncFunc = func;
            return (IOperationAsyncBuilder)builder;
        }

        /// <summary>
        /// Set an async callback to be execute after the operation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="postExecute"></param>
        /// <returns></returns>
        public static IOperationAsyncBuilder SetPostExecute(this IOperationAsyncBuilder builder, Func<Task> postExecute)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.AsyncPostExecute = postExecute;
            return builder;
        }

        /// <summary>
        /// Set a sync callback to be execute after the operation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="postExecute"></param>
        /// <returns></returns>
        public static IOperationSyncBuilder SetPostExecute(this IOperationSyncBuilder builder, Action postExecute)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.OperationDetails.PostExecute = postExecute;
            return builder;
        }
    }
}
