using Autofac;
using Conreign.Core.Contracts.Game;
using Conreign.Framework;
using Conreign.Framework.Http;
using Conreign.Framework.Http.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Conreign.Api.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            var container = CreateContainer();
            builder.SetFrameworkLoggerFactory();
            builder.UseCors(CorsOptions.AllowAll);
            builder.UseAutofacMiddleware(CreateContainer());
            builder.UseFrameworkErrorHandler();
            builder.MapFrameworkDispatcher();
            builder.MapFrameworkEventHub(container);
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            var configuration = new FrameworkConfiguration
            {
                GrainContractsAssembly = typeof (IWorldGrain).Assembly
            };
            builder.RegisterModule(new FrameworkModule(configuration));

            var httpConfiguration = new HttpFrameworkConfiguration
            {
                SerializerSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}
            };
            builder.RegisterModule(new HttpFrameworkModule(httpConfiguration));

            return builder.Build();
        }
    }
}