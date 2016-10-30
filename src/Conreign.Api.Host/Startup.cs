using Conreign.Api.Configuration;
using Conreign.Client.Orleans;
using Microsoft.Owin.Cors;
using Orleans.Runtime.Configuration;
using Owin;

namespace Conreign.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
            var config = ClientConfiguration.LoadFromFile("OrleansClientConfiguration.xml");
            var host = new OrleansClientInitializer(config);
            var options = new ConreignApiOptions(host);
            builder.MapConreignApi(options);
        }
    }
}