using Microsoft.Owin.Cors;
using Owin;

namespace Conreign.Api.Host.Azure
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