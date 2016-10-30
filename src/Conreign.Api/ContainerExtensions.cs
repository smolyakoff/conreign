using System;
using System.Linq;
using System.Reflection;
using Conreign.Api.Configuration;
using Conreign.Client.Handler;
using Conreign.Client.Orleans;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;
using SimpleInjector;

namespace Conreign.Api
{
    public static class ContainerExtensions
    {
        public static Container RegisterConreignApi(this Container container, ConreignApiOptions options)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            container.Register(() => Log.Logger, Lifestyle.Singleton);
            container.Register(() => OrleansClient.Initialize(options.OrleansClientInitializer).Result,
                Lifestyle.Singleton);
            container.RegisterCollection<HubPipelineModule>(new[] {Assembly.GetExecutingAssembly()});
            RegisterHubs(container);
            container.RegisterClientMediator();
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
                container.Register(hubType);
            }
        }
    }
}