using System;
using Conreign.Api.Configuration;
using Conreign.Api.Infrastructure;
using Conreign.Client.Orleans;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;
using SimpleInjector;

namespace Conreign.Api
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder MapConreignApi(
            this IAppBuilder builder, 
            IOrleansClientInitializer initializer, 
            ConreignApiConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (initializer == null)
            {
                throw new ArgumentNullException(nameof(initializer));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var container = new Container();
            container.RegisterConreignApi(initializer, configuration);
            var hubConfiguration = ConfigureSignalR(container);
            builder.UseWelcomePage("/");
            builder.MapSignalR(configuration.Path, hubConfiguration);
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