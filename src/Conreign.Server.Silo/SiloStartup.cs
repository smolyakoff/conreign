using System;
using Conreign.Server.Gameplay;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Conreign.Server.Silo
{
    internal class SiloStartup
    {
        public static ConreignSiloConfiguration Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
            services.AddSingleton(new GameGrainOptions());
            services.AddSingleton(new LobbyGrainOptions());
            return services.BuildServiceProvider();
        }
    }
}
