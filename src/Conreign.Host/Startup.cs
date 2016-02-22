using System;
using System.Linq;
using Conreign.Core.Auth;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Game;
using Microsoft.Extensions.DependencyInjection;
using Orleans;

namespace Conreign.Host
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddInstance(new AuthOptions
            {
                JwtSecret = "conreign-debug"
            });
            var grains = typeof (WorldGrain).Assembly.DefinedTypes
                .Where(t => t.IsClass)
                .Where(t => typeof (Grain).IsAssignableFrom(t))
                .ToList();
            foreach (var grain in grains)
            {
                var interfaces = grain.GetInterfaces()
                    .Where(t => typeof(IGrain).IsAssignableFrom(t))
                    .Where(t => t != typeof(IGrain))
                    .Where(t => !t.Name.EndsWith("Key"));
                services.AddTransient(grain, grain);
                foreach (var @interface in interfaces)
                {
                    services.AddTransient(@interface, grain);
                }
            }
            var provider = services.BuildServiceProvider();
            return provider;
        }

        
    }
}
