using System;
using System.Net;
using System.Threading;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Serilog;

namespace Conreign.Api.Host.Azure
{
    public class OwinRole : RoleEntryPoint
    {
        private IDisposable _app;
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);

        public override void Run()
        {
            _exitEvent.WaitOne();
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var storage = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.AzureTableStorage(storage)
                .WriteTo.Trace()
                .CreateLogger();

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["PublicApi"];
            // Don't know why azure doesn't provide private port
            var baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";
            _app = WebApp.Start<Startup>(baseUri);
            Log.Logger.Information("Conreign API is started at {BaseUri}", baseUri);
            return base.OnStart();
        }

        public override void OnStop()
        {
            _app?.Dispose();
            base.OnStop();
        }
    }
}
