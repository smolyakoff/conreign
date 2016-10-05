using Autofac;
using Microsoft.Owin.Cors;
using Owin;

namespace Conreign.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var container = CreateContainer();
            builder.UseCors(CorsOptions.AllowAll);
            builder.UseAutofacMiddleware(CreateContainer());
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            return builder.Build();
        }
    }
}