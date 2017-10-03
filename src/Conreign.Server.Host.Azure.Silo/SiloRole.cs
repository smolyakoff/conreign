using System;
using Conreign.Server.Silo;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orleans.Runtime.Host;

namespace Conreign.Server.Host.Azure.Silo
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
            config.ClusterId = RoleEnvironment.DeploymentId;
            config.InstanceId = RoleEnvironment.CurrentRoleInstance.Id;
            var app = ConreignSilo.Create(config);
            var orleansConfiguration = app.CreateOrleansConfiguration();
            _silo = new AzureSilo();
            var started = _silo.Start(orleansConfiguration, app.Configuration.SystemStorageConnectionString);
            if (!started)
            {
                throw new InvalidOperationException("Silo was not started.");
            }
            _silo.Run();
        }

        public override void OnStop()
        {
            _silo.Stop();
            base.OnStop();
        }
    }
}