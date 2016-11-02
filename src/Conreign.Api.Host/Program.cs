using System;
using Microsoft.Owin.Hosting;
using Serilog;

namespace Conreign.Api.Host
{
    internal class Program
    {
        private const string Url = "http://localhost:9000/";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Debug()
                .CreateLogger()
                .ForContext("ApplicationId", "Conreign.Api");
            try
            {
                using (RunOwin())
                {
                    Console.WriteLine("Press any key to stop API host.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Api server cannot start. {Message}", ex.Message);
                Console.ReadLine();
            }
        }

        private static IDisposable RunOwin()
        {
            Console.WriteLine("Starting Web API...");
            var app = WebApp.Start<Startup>(Url);
            Console.WriteLine($"Web API is running at {Url}");
            return app;
        }
    }
}