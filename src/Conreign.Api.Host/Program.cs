using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Orleans;
using Polly;
using Serilog;

namespace Conreign.Api.Host
{
    internal class Program
    {
        private const string Url = "http://localhost:9000/";

        private const string ConfigFileName = "OrleansClientConfiguration.xml";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();
            var tasks = new []
            {
                Task.Run(() => RunOwin()),
                Task.Run(() => RunOrleans())
            };
            IDisposable[] apps;
            try
            {
                apps = Task.WhenAll(tasks).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical problem: {Environment.NewLine}{ex}");
                return;
            }
            Console.WriteLine("Press any key to stop API host.");
            Console.ReadLine();
            foreach (var app in apps)
            {
                try
                {
                    app.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while stopping host: {Environment.NewLine}{ex}");
                }
            }
        }

        private static IDisposable RunOwin()
        {
            Console.WriteLine("Starting Web API...");
            var app = WebApp.Start<Startup>(Url);
            Console.WriteLine($"Web API is running at {Url}...");
            return app;
        }

        private static IDisposable RunOrleans()
        {
#if DEBUG
            //Wait for orleans silo to start
            Thread.Sleep(5000);
#endif
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(attempt*3));
            Console.WriteLine("Trying to initialize Orleans client...");
            policy.Execute(() => GrainClient.Initialize(ConfigFileName));
            return new OrleansClientDisposer();
        }

        private class OrleansClientDisposer : IDisposable
        {
            public void Dispose()
            {
                if (GrainClient.IsInitialized)
                {
                    GrainClient.Uninitialize();
                }
            }
        }
    }
}
