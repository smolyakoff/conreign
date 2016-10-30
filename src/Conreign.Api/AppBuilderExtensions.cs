using System;
using System.Runtime.InteropServices.ComTypes;
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
            container.RegisterConreignApi(options);
            var hubConfiguration = ConfigureSignalR(container);
            builder.UseWelcomePage("/");
            builder.MapSignalR(options.Path, hubConfiguration);
            return builder;
        }

        private static HubConfiguration ConfigureSignalR(Container container)
        {
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };
            hubConfiguration.Resolver.Register(typeof(IHubActivator), () => new SimpleInjectorHubActivator(container));
            var pipeline = hubConfiguration.Resolver.Resolve<IHubPipeline>();
            foreach (var module in container.GetAllInstances<HubPipelineModule>())
            {
                pipeline.AddModule(module);
            }

            return hubConfiguration;
        }
    }
}