using System;
using System.Net;
using System.Threading;
using Conreign.Api.Configuration;
using Microsoft.Azure;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Conreign.Api.Host.Azure
{
    public class OwinRole : RoleEntryPoint
    {
        private IDisposable _app;
        private IDisposable _serilogDisposable;
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);


        public static ConreignApi Api { get; private set; }

        public override void Run()
        {
            _exitEvent.WaitOne();
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 20;
            var env = CloudConfigurationManager.GetSetting("Environment");
            if (string.IsNullOrEmpty(env))
            {
                throw new InvalidOperationException("Unable to determine environment.");
            }
            var config = ConreignApiConfiguration.Load(Environment.CurrentDirectory, env);
            var clientConfiguration = new ClientConfiguration {DeploymentId = RoleEnvironment.DeploymentId};
            Api = ConreignApi.Create(clientConfiguration, config);
            Log.Logger = Api.Logger;
            ILogEventEnricher[] logProperties = {
                new PropertyEnricher("DeploymentId", RoleEnvironment.DeploymentId),
                new PropertyEnricher("InstanceId", RoleEnvironment.CurrentRoleInstance.Id),
            };
            _serilogDisposable = LogContext.PushProperties(logProperties);
            // Starting appplication
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["PublicApi"];
            var baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";
            _app = WebApp.Start<Startup>(baseUri);
            Log.Logger.Information("Conreign API ({Environment}) is started at {BaseUri}", Api.Configuration.Environment, baseUri);
            return base.OnStart();
        }

        public override void OnStop()
        {
            _app?.Dispose();
            _serilogDisposable?.Dispose();
            base.OnStop();
        }
    }
}
