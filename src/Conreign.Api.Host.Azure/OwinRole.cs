using System;
using System.Net;
using System.Threading;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using Serilog.Sinks.Elasticsearch;

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
            ServicePointManager.DefaultConnectionLimit = 20;
            var elasticOptions = new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                AutoRegisterTemplate = true,
                BufferBaseFilename = "logs/elastic-buffer"
            };
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(elasticOptions)
                .WriteTo.Trace()
                .CreateLogger()
                .ForContext("DeploymentId", RoleEnvironment.DeploymentId)
                .ForContext("InstanceId", RoleEnvironment.CurrentRoleInstance.Id)
                .ForContext("ApplicationId", "Conreign.Api");
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["PublicApi"];
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
