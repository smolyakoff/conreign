using Conreign.Api.Framework.Owin;
using Microsoft.Owin.Cors;
using Owin;

namespace Conreign.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var container = Api.Configuration.Container;
            var hubConfiguration = Api.Configuration.CreateHubConfiguration();
            var frameworkOptions = Api.Configuration.CreateFrameworkOptions();

            builder.SetFrameworkLoggerFactory();
            builder.UseCors(CorsOptions.AllowAll);
            builder.UseAutofacMiddleware(container);
            builder.UseFrameworkErrorHandler(frameworkOptions);
            builder.MapSignalR(hubConfiguration);
            builder.MapFrameworkDispatcher(frameworkOptions);
        }
    }
}
