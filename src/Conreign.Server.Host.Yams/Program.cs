using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Permissions;
using Etg.Yams;
using Serilog;
using Serilog.Events;
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
                var configurationDirectory = ApplicationPath.CurrentDirectory;
                host.AddCommandLineDefinition("config", x => configurationDirectory = x);
                host.ApplyCommandLine();
                var configuration = YamsServiceConfiguration.Load(configurationDirectory);
                var logger = new LoggerConfiguration()
                    .WriteTo.RollingFile(Path.Combine(ApplicationPath.CurrentDirectory, "logs", "yams-{Date}.log"))
                    .MinimumLevel.Is(LogEventLevel.Debug)
                    .CreateLogger();
                Trace.Listeners.Add(new SerilogTraceListener.SerilogTraceListener(logger));

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
