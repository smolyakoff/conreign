using Conreign.Api.Configuration;
using Conreign.Core.Contracts.Communication;
using Microsoft.Owin.Cors;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Owin;

namespace Conreign.Api.Host.Azure
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseCors(CorsOptions.AllowAll);
            var options = ConfigureApi();
            builder.MapConreignApi(options);
        }

        private static ConreignApiOptions ConfigureApi()
        {
            var config = new ClientConfiguration
            {
                DataConnectionString =
                    RoleEnvironment.GetConfigurationSettingValue("OrleansSystemStorageConnectionString"),
                GatewayProvider = ClientConfiguration.GatewayProviderType.AzureTable,
                DeploymentId = RoleEnvironment.DeploymentId
            };
            config.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);
            var host = new OrleansAzureClientInitializer(config);
            var options = new ConreignApiOptions(host);
            return options;
        }
    }
}