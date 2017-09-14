using System;
using System.Net;
using System.Security.Permissions;
using Etg.Yams;
using Topshelf;

namespace Conreign.Server.Host.Yams
{
    internal class Program
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
            HostFactory.Run(host =>
            {
                var configurationDirectory = Environment.CurrentDirectory;
                var environment = YamsServiceConfiguration.DefaultEnvironment;

                host.AddCommandLineDefinition("config", x => configurationDirectory = x);
                host.AddCommandLineDefinition("env", x => environment = x);
                host.ApplyCommandLine();
                var configuration = YamsServiceConfiguration.Load(configurationDirectory, environment);

                host.Service<IYamsService>(service =>
                {
                    service.ConstructUsing(name =>
                    {
                        var builder = new YamsConfigBuilder(
                            configuration.ClusterId,
                            configuration.InstanceUpdateDomain,
                            configuration.InstanceUpdateDomain,
                            configuration.ApplicationInstallDirectory);
                        var yamsConfig = builder
                            .SetCheckForUpdatesPeriodInSeconds(configuration.UpdatePeriodInSeconds)
                            .SetApplicationRestartCount(configuration.RestartCount)
                            .Build();
                        return YamsServiceFactory.Create(
                            yamsConfig, 
                            configuration.AzureStorageConnectionString, 
                            configuration.AzureStorageConnectionString);
                    });
                    service.WhenStarted(x => x.Start().Wait());
                    service.WhenStopped(x => x.Stop().Wait());
                });

                host.RunAsLocalSystem();
                host.SetDescription("Yams Service Host");
                host.SetDisplayName("Yams Service");
                host.SetServiceName("Yams");
                host.StartAutomatically();
            });
        }
    }
}
