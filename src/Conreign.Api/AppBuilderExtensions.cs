using System;
using System.Collections.Generic;
using System.Reflection;
using Conreign.Api.Configuration;
using Conreign.Api.Infrastructure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;
using SimpleInjector;

namespace Conreign.Api
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder MapConreignApi(this IAppBuilder builder, ConreignApiOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var container = new Container();
            var apiPackage = new ConreignApiPackage(options);
            apiPackage.RegisterServices(container);
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };
            hubConfiguration.Resolver.Register(typeof(IHubActivator), () => new SimpleInjectorHubActivator(container));
            builder.MapSignalR(options.Path, hubConfiguration);
            return builder;
        }
    }
}
