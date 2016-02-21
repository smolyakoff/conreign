using System;
using Microsoft.Owin.Logging;
using Owin;

namespace Conreign.Api.Framework.Owin
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

        public static IAppBuilder MapFrameworkDispatcher(this IAppBuilder builder, FrameworkOptions options, string url = "/dispatch")
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            url = string.IsNullOrEmpty(url) ? "/dispatch" : url;
            builder.Map(url, b => b.Use<FrameworkDispatcherMiddleware>(options));
            return builder;
        }

        public static IAppBuilder UseFrameworkErrorHandler(this IAppBuilder builder, FrameworkOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            builder.Use<FrameworkErrorHandlerMiddleware>(options);
            return builder;
        }
    }
}
