using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Features.Variance;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using Conreign.Framework.Core.Serialization;
using Conreign.Framework.Routing;
using MediatR;

namespace Conreign.Framework
{
    public class FrameworkModule : Module
    {
        private readonly FrameworkConfiguration _configuration;

        public FrameworkModule(FrameworkConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            configuration.EnsureIsValid();
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterInstance(Console.Out).As<TextWriter>();
            builder.RegisterType<Mediator>().As<IMediator>().SingleInstance();

            var handlerType = _configuration.Pipeline.Last();
            var decoratorTypes = _configuration.Pipeline.AsEnumerable().Reverse().Skip(1).ToList();
            var handlerInterfaceType = typeof (IAsyncRequestHandler<Request, Response>);

            // Register request handler
            builder.RegisterType(handlerType)
                .Named<IAsyncRequestHandler<Request, Response>>(handlerType.AssemblyQualifiedName)
                .SingleInstance();

            var from = handlerType.AssemblyQualifiedName;

            // Register request handler pipeline steps
            for (var i = 0; i < decoratorTypes.Count; i++)
            {
                var decoratorType = decoratorTypes[i];
                var isLast = i == decoratorTypes.Count - 1;
                var to = isLast ? null : decoratorType.AssemblyQualifiedName;
                builder.RegisterType(decoratorType).AsSelf().SingleInstance();
                builder.RegisterDecorator<IAsyncRequestHandler<Request, Response>>(
                    (c, next) =>
                        (IAsyncRequestHandler<Request, Response>)
                            c.Resolve(decoratorType, new TypedParameter(handlerInterfaceType, next)),
                    from,
                    to);
                from = to;
            }

            // Register optional event handler
            if (!string.IsNullOrEmpty(_configuration.StreamProviderName))
            {
                builder.RegisterType<EventHandler>()
                    .AsImplementedInterfaces()
                    .WithParameter(new PositionalParameter(0, _configuration.StreamProviderName))
                    .SingleInstance();
            }

            builder.RegisterInstance(new RouteTable(_configuration.GrainContractsAssembly));
            builder.RegisterInstance(_configuration.Converter).As<IConverter>();
        }
    }
}