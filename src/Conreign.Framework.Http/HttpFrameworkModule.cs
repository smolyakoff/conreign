using System;
using Autofac;
using Autofac.Integration.SignalR;
using Conreign.Framework.Http.Core;
using Conreign.Framework.Http.Core.Data;
using MediatR;

namespace Conreign.Framework.Http
{
    public class HttpFrameworkModule : Module
    {
        private readonly HttpFrameworkConfiguration _configuration;

        public HttpFrameworkModule(HttpFrameworkConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterInstance(_configuration.SerializerSettings);
            builder.RegisterInstance(_configuration.ErrorFactorySettings);
            builder.RegisterInstance(_configuration.EventHubSettings);

            builder.RegisterType<HttpActionHandler>()
                .As<IAsyncRequestHandler<HttpAction, HttpActionResult>>()
                .SingleInstance();
            builder.RegisterHubs(typeof (EventHub).Assembly);
        }
    }
}