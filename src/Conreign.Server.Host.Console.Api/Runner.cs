using System;
using Conreign.Server.Api;
using Conreign.Server.Api.Configuration;
using Microsoft.Owin.Hosting;
using Orleans.Runtime.Configuration;
using Serilog;

namespace Conreign.Server.Host.Console.Api
{
    public static class Runner
    {
        public static IDisposable Run(string[] args)
        {
            var apiConfiguration = ConreignApiConfiguration.Load(ApplicationPath.CurrentDirectory, args);
            var api = ConreignApi.Create(ClientConfiguration.LocalhostSilo(), apiConfiguration);
            var logger = api.Logger;
            Log.Logger = logger;
            try
            {
                logger.Information("Starting API...");
                var url = $"http://*:{apiConfiguration.Port}";
                Startup.Api = api;
                var webApp = WebApp.Start<Startup>(url);
                logger.Information($"API is running at {url}.");
                return webApp;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Failed to start API.");
                throw;
            }
        }
    }
}