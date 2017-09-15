using Conreign.Client.Orleans;
using Conreign.Server.Api;
using Microsoft.Owin.Cors;
using Owin;

namespace Conreign.Server.Host.Console.Api
{
    public class Startup
    {
        public static ConreignApi Api { get; set; }

        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
 
            var initializer = new OrleansClientInitializer(Api.OrleansConfiguration);
            builder.MapConreignApi(initializer, Api.Configuration);
        }
    }
}