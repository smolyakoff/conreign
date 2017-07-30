using System;
using System.Linq;
using System.Reflection;
using Conreign.Api.Configuration;
using Conreign.Api.Hubs;
using Conreign.Client.Handler;
using Conreign.Client.Handler.Behaviours;
using Conreign.Client.Orleans;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;
using SimpleInjector;

namespace Conreign.Api
{
    public static class ContainerExtensions
    {
        public static Container RegisterConreignApi(this Container container, IOrleansClientInitializer initializer, ConreignApiConfiguration configuration)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (initializer == null)
            {
                throw new ArgumentNullException(nameof(initializer));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            container.Register(() => Log.Logger.ForContext("ApplicationId", "Conreign.Api"), Lifestyle.Singleton);
            container.Register(() => OrleansClient.Initialize(initializer).Result, Lifestyle.Singleton);
            container.Register<GameHubCountersCollection>(Lifestyle.Singleton);
            container.RegisterCollection<HubPipelineModule>(new[] {Assembly.GetExecutingAssembly()});
            RegisterHubs(container);
            container.RegisterSingleton(new SlowConnectionBehaviourOptions(TimeSpan.FromSeconds(3)));
            container.RegisterClientHandlerFactory(c =>
            {
                // c.Behaviours.Add(typeof(SlowConnectionBehaviour<,>));
            });
            return container;
        }

        private static void RegisterHubs(Container container)
        {
            var hubTypes = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(t => typeof(Hub).IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();
            foreach (var hubType in hubTypes)
            {
                container.Register(hubType, hubType, Lifestyle.Scoped);
            }
        }
    }
}