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
            var orleansConfiguration = OwinRole.Api.CreateOrleansConfiguration();
            var initializer = new OrleansAzureClientInitializer(orleansConfiguration);
            builder.MapConreignApi(initializer, OwinRole.Api.Configuration);
        }
    }
}