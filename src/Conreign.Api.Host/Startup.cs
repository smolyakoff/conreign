using Conreign.Api.Configuration;
using Owin;

namespace Conreign.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var config = ApiConfiguration.CreateHttpConfiguration();

            builder.UseWebApi(config);
        }
    }
}
