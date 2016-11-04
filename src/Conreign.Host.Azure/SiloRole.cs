using System;
using Conreign.Cluster;
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
            var env = RoleEnvironment.GetConfigurationSettingValue("Environment");
            if (string.IsNullOrEmpty(env))
            {
                throw new InvalidOperationException("Unable to determine environment.");
            }
            var config = ConreignSiloConfiguration.Load(Environment.CurrentDirectory, env);
            var orleansConfiguration = new ClusterConfiguration();
            orleansConfiguration.Globals.DeploymentId = RoleEnvironment.DeploymentId;
            var conreignSilo = ConreignSilo.Configure(orleansConfiguration, config);
            _silo = new AzureSilo();
            var started = _silo.Start(conreignSilo.OrleansConfiguration, conreignSilo.Configuration.SystemStorageConnectionString);
            if (!started)
            {
                throw new InvalidOperationException("Silo was not started.");
            }
            conreignSilo.Initialize();
            _silo.Run();
        }

        public override void OnStop()
        {
            _silo.Stop();
            base.OnStop();
        }
    }
}