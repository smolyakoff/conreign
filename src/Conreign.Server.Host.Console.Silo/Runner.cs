using System;
using Conreign.Server.Silo;
using Orleans.Runtime.Host;

namespace Conreign.Server.Host.Console.Silo
{
    public static class Runner
    {
        public static IDisposable Run(string[] args)
        {
            var configuration = ConreignSiloConfiguration.Load(
                ApplicationPath.CurrentDirectory,
                args
            );
            var silo = ConreignSilo.Create(configuration);
            var logger = silo.Logger;
            var orleansConfiguration = silo.CreateOrleansConfiguration();
            var siloHost = new SiloHost(Environment.MachineName, orleansConfiguration);
            try
            {
                siloHost.DeploymentId = configuration.ClusterId;
                siloHost.InitializeOrleansSilo();
                siloHost.StartOrleansSilo(false);
                logger.Information("Silo is started.");
                return siloHost;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to start silo.");
                siloHost.Dispose();
                throw;
            }
        }
    }
}