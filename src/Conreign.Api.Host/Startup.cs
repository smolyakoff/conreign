using Conreign.Api.Configuration;
using Microsoft.Owin.Cors;
using Owin;

namespace Conreign.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
            var options = new ConreignApiOptions
            {
                OrleansClientConfigFilePath = "OrleansClientConfiguration.xml"
            };
            builder.MapConreignApi(options);
        }
    }
}