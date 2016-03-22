using System;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Logging;
using Owin;

namespace Conreign.Framework.Http.Owin
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder SetFrameworkLoggerFactory(this IAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.SetLoggerFactory(new SerilogOwinLoggerFactory());
            return builder;
        }

        public static IAppBuilder MapFrameworkDispatcher(this IAppBuilder builder, string url = "/dispatch")
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            url = string.IsNullOrEmpty(url) ? "/dispatch" : url;
            builder.Map(url, b => b.Use<FrameworkDispatcherMiddleware>());
            return builder;
        }

        public static IAppBuilder MapFrameworkEventHub(this IAppBuilder builder, ILifetimeScope container, string url = "/events")
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            url = string.IsNullOrEmpty(url) ? "/events" : url;
            var config = new HubConfiguration
            {
                EnableJavaScriptProxies = false,
                Resolver = new AutofacDependencyResolver(container)
            };
            builder.MapSignalR(url, config);
            return builder;
        }

        public static IAppBuilder UseFrameworkErrorHandler(this IAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Use<FrameworkErrorHandlerMiddleware>();
            return builder;
        }
    }
}