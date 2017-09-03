using Conreign.Server.Api;
using Microsoft.Owin.Cors;
using Owin;

namespace Conreign.Server.Host.Azure.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
            var initializer = new OrleansAzureClientInitializer(OwinRole.Api.OrleansConfiguration);
            builder.MapConreignApi(initializer, OwinRole.Api.Configuration);
        }
    }
}