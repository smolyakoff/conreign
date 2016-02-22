using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Features.Variance;
using Autofac.Integration.SignalR;
using Conreign.Api.Framework;
using Conreign.Api.Framework.Auth;
using Conreign.Api.Framework.Diagnostics;
using Conreign.Api.Framework.Routing;
using Conreign.Core.Contracts.Abstractions;
using MediatR;
using Module = Autofac.Module;

namespace Conreign.Api
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            // SignalR
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            // MediatR
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterInstance(Console.Out)
                .As<TextWriter>();

            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .SingleInstance();
            builder.RegisterInstance(new RoutingTable(typeof (IGrainAction<>).Assembly));
            builder.RegisterType<ActionHandler>()
                .Named<IAsyncRequestHandler<GenericAction, GenericActionResult>>("ActionHandler")
                .SingleInstance();
            builder.RegisterDecorator<IAsyncRequestHandler<GenericAction, GenericActionResult>>(
                (c, next) => new AuthDecorator(next),
                fromKey: "ActionHandler", toKey: "AuthenticatedActionHandler");
            builder.RegisterDecorator<IAsyncRequestHandler<GenericAction, GenericActionResult>>(
                (c, next) => new LoggingDecorator(next),
                fromKey: "AuthenticatedActionHandler").SingleInstance();

            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>) c.Resolve(typeof (IEnumerable<>).MakeGenericType(t));
            });
        }
    }
}
