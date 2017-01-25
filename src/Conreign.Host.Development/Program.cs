using System;
using System.Net;
using System.Security.Policy;
using Conreign.Api;
using Conreign.Api.Configuration;
using Conreign.Cluster;
using Microsoft.Owin.Hosting;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Serilog;

namespace Conreign.Host.Development
{
    internal class Program
    {
        private static SiloHost _silo;
        private const string SiloAppDomainName = "OrleansSilo";

        public static void Main(string[] args)
        {
            using (RunSilo())
            using (RunApi())
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }

        }

        private static IDisposable RunApi()
        {
            ServicePointManager.DefaultConnectionLimit = 20;
            var config = ConreignApiConfiguration.Load();
            var api = ConreignApi.Create(new ClientConfiguration(), config);
            Log.Logger = api.Logger;
            Log.Logger.Information("Starting API...");
            var url = $"http://localhost:{config.Port}";
            var app = WebApp.Start<Startup>(url);
            Log.Logger.Information($"API is running at {url}.");
            return app;
        }

        private static IDisposable RunSilo()
        {
            var domain = AppDomain.CreateDomain(SiloAppDomainName,
    null,
    new AppDomainSetup { AppDomainInitializer = InitializeSilo });
            Console.WriteLine("Orleans silo is running.");
            return new Disposable(() => domain.DoCallBack(ShutdownSilo));
        }

        private static void InitializeSilo(string[] args)
        {
            var cluster = ConreignSilo.Configure(ClusterConfiguration.LocalhostPrimarySilo());
            _silo = new SiloHost(Dns.GetHostName(), cluster.OrleansConfiguration);
            _silo.InitializeOrleansSilo();
            var started = _silo.StartOrleansSilo();
            if (started)
            {
                cluster.Initialize();
                return;
            }
            var message = $"Failed to start Orleans silo '{_silo.Name}' as a {_silo.Type}.";
            throw new SystemException(message);
        }

        private static void ShutdownSilo()
        {
            if (_silo == null)
            {
                return;
            }
            _silo.Dispose();
            GC.SuppressFinalize(_silo);
            _silo = null;
        }


    }
}
