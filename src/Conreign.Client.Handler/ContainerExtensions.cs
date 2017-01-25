using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Conreign.Client.Handler.Handlers;
using Conreign.Client.Handler.Handlers.Behaviours;
using MediatR;
using SimpleInjector;

namespace Conreign.Client.Handler
{
    public static class ContainerExtensions
    {
        public static void RegisterClientMediator(this Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            var assembly = typeof(LoginHandler).Assembly;
            var assemblies = new List<Assembly> {assembly};
            container.Register(typeof(IAsyncRequestHandler<,>), assemblies, Lifestyle.Singleton);
            container.RegisterCollection(typeof(IPipelineBehavior<,>), new List<Type>
            {
                typeof(DiagnosticsBehaviour<,>),
                typeof(ErrorLoggingBehaviour<,>),
                typeof(AuthenticationBehaviour<,>)           
            });
            container.Register(() =>
            {
                var configuration = new MapperConfiguration(cfg => cfg.AddProfiles(assemblies));
                return configuration.CreateMapper();
            }, Lifestyle.Singleton);
            container.Register<SingleInstanceFactory>(() => container.GetInstance, Lifestyle.Singleton);
            container.Register<MultiInstanceFactory>(() => container.GetAllInstances, Lifestyle.Singleton);
            container.Register<IMediator, Mediator>(Lifestyle.Singleton);
        }
    }
}