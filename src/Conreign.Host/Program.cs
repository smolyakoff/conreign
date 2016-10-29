using System;
using System.Net;
using Conreign.Core.Gameplay;
using Conreign.Host.Storage;
using Orleans.Runtime.Host;

namespace Conreign.Host
{
    internal class Program
    {
        private const string AppDomainName = "OrleansHost";
        private const string ConfigFileName = "OrleansServerConfiguration.xml";
        private static SiloHost _host;

        public static void Main(string[] args)
        {
            var domain = AppDomain.CreateDomain(AppDomainName,
                null,
                new AppDomainSetup {AppDomainInitializer = InitializeSilo});
            Console.WriteLine("Orleans silo is running... Press any key to terminate.");
            Console.ReadLine();

            domain.DoCallBack(ShutdownSilo);
        }

        private static void InitializeSilo(string[] args)
        {
            var assembly = typeof(GameGrain).Assembly;
            Console.WriteLine($"Loaded grain assembly: {assembly.GetName().Name}");
            _host = new SiloHost(Dns.GetHostName())
            {
                ConfigFileName = ConfigFileName
            };
            _host.InitializeOrleansSilo();
            var started = _host.StartOrleansSilo();
            if (started)
            {
                Initialize();
                return;
            }
            var message = $"Failed to start Orleans silo '{_host.Name}' as a {_host.Type}.";
            throw new SystemException(message);
        }

        private static void ShutdownSilo()
        {
            if (_host == null)
            {
                return;
            }
            _host.Dispose();
            GC.SuppressFinalize(_host);
            _host = null;
        }

        private static void Initialize()
        {
            MongoDriverConfiguration.EnsureInitialized();
        }
    }
}