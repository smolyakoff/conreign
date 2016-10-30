using System;
using System.Diagnostics;
using Conreign.Core.Contracts.Communication;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace Conreign.Host.Azure
{
    public class SiloRole : RoleEntryPoint
    {
        private AzureSilo _silo;

        public override void Run()
        {
            var config = new ClusterConfiguration();
            config.AddMemoryStorageProvider();
            config.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);

            _silo = new AzureSilo {DataConnectionConfigurationSettingName = "OrleansSystemStorageConnectionString"};
            var started = _silo.Start(config);
            if (!started)
            {
                throw new InvalidOperationException("Silo was not started.");
            }
            _silo.Run();
        }

        public override bool OnStart()
        {
            return base.OnStart();
        }

        public override void OnStop()
        {
            _silo.Stop();
            base.OnStop();
        }
    }
}