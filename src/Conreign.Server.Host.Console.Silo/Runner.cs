using System;
using Conreign.Server.Silo;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace Conreign.Server.Host.Console.Silo
{
    public static class Runner
    {
        public static IDisposable Run(string[] args)
        {
            var conreignConfiguration = ConreignSiloConfiguration.Load(
                Environment.CurrentDirectory,
                args
            );
            var orleansConfiguration = ClusterConfiguration.LocalhostPrimarySilo();
            var silo = ConreignSilo.Create(orleansConfiguration, conreignConfiguration);
            var logger = silo.Logger;
            var siloHost = new SiloHost(Environment.MachineName, silo.OrleansConfiguration);
            try
            {
                siloHost.DeploymentId = conreignConfiguration.ClusterId;
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