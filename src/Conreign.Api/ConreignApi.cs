using System;
using System.Linq;
using System.Reflection;
using Conreign.Api.Configuration;
using Conreign.Core.Client;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;
using SimpleInjector;

namespace Conreign.Api
{
    public class ConreignApi
    {
        private readonly ConreignApiOptions _options;

        public ConreignApi(ConreignApiOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
        }

        public void RegisterServices(Container container)
        {
            container.Register(() => Log.Logger, Lifestyle.Singleton);
            container.Register(() => OrleansGameClient.Initialize(_options.OrleansClientConfigFilePath).Result, Lifestyle.Singleton);
            container.RegisterCollection<HubPipelineModule>(new[] {Assembly.GetExecutingAssembly()});
            RegisterHubs(container);
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
