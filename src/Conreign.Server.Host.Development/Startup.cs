using Conreign.Client.Orleans;
using Conreign.Server.Api;
using Conreign.Server.Api.Configuration;
using Microsoft.Owin.Cors;
using Orleans.Runtime.Configuration;
using Owin;
using Serilog;

namespace Conreign.Server.Host.Development
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Debug()
                .CreateLogger()
                .ForContext("ApplicationId", "Conreign.Api");
            builder.UseCors(CorsOptions.AllowAll);
            var api = ConfigureApi();
            var initializer = new OrleansClientInitializer(api.OrleansConfiguration);
            builder.MapConreignApi(initializer, api.Configuration);
        }

        private static ConreignApi ConfigureApi()
        {
            var apiConfiguration = ConreignApiConfiguration.Load();
            var api = ConreignApi.Create(ClientConfiguration.LocalhostSilo(), apiConfiguration);
            return api;
        }
    }
}