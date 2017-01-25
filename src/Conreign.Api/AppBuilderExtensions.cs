using System;
using System.Linq;
using Conreign.Api.Configuration;
using Conreign.Api.Infrastructure;
using Conreign.Client.Orleans;
using Conreign.Core.Contracts.Client.Messages;
using MediatR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Owin;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

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
            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            container.RegisterConreignApi(initializer, configuration);
            var hubConfiguration = ConfigureSignalR(container);
            builder.UseWelcomePage("/");
            builder.MapSignalR<SimpleInjectorHubDispatcher>(configuration.Path, hubConfiguration);
            return builder;
        }

        private static HubConfiguration ConfigureSignalR(Container container)
        {
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCaseForNonSignalRTypesResolver()
            };
            var serializer = JsonSerializer.Create(serializerSettings);
            hubConfiguration.Resolver.Register(typeof(JsonSerializer), () => serializer);
            hubConfiguration.Resolver.Register(typeof(SimpleInjectorHubDispatcher),
                () => new SimpleInjectorHubDispatcher(container, hubConfiguration));
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