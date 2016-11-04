using System;
using System.Net;
using Conreign.Api.Configuration;
using Microsoft.Owin.Hosting;
using Orleans.Runtime.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Conreign.Api.Host
{
    internal class Program
    {
        private const string Url = "http://localhost:9000/";

        public static ConreignApi Api { get; private set; }

        public static void Main(string[] args)
        {
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
            ServicePointManager.DefaultConnectionLimit = 20;
            var config = ConreignApiConfiguration.Load();
            Api = ConreignApi.Configure(new ClientConfiguration(), config);
            Log.Logger = Api.Logger;
            Log.Logger.Information("Starting Web API...");
            var app = WebApp.Start<Startup>(Url);
            Log.Logger.Information($"Web API is running at {Url}");
            return app;
        }
    }
}