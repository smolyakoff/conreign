using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using Conreign.Client.Handler.Handlers;
using MediatR;
using SimpleInjector;

namespace Conreign.Client.Handler
{
    public static class ContainerExtensions
    {
        public static Container RegisterClientHandlerFactory(this Container container)
        {
            return container.RegisterClientHandlerFactory(o => { });
        }

        public static Container RegisterClientHandlerFactory(this Container container,
            Action<ClientHandlerFactoryOptions> configure)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            var options = new ClientHandlerFactoryOptions();
            configure(options);
            var assembly = typeof(LoginHandler).Assembly;
            var assemblies = new List<Assembly> {assembly};
            container.Register(typeof(IAsyncRequestHandler<,>), assemblies, Lifestyle.Singleton);
            var behaviourTypes = container.GetTypesToRegister(typeof(IPipelineBehavior<,>), assemblies,
                new TypesToRegisterOptions {IncludeGenericTypeDefinitions = true});
            foreach (var type in behaviourTypes)
                container.RegisterSingleton(type, type);
            container.RegisterCollection(typeof(IPipelineBehavior<,>), options.Behaviours);
            container.Register(() =>
            {
                var configuration = new MapperConfiguration(cfg => cfg.AddProfile<HandlersMappingProfile>());
                return configuration.CreateMapper();
            }, Lifestyle.Singleton);
            container.Register<SingleInstanceFactory>(() => container.GetInstance, Lifestyle.Singleton);
            container.Register<MultiInstanceFactory>(() => container.GetAllInstances, Lifestyle.Singleton);
            container.Register<IMediator, Mediator>(Lifestyle.Singleton);
            container.Register<ClientHandlerFactory>(Lifestyle.Singleton);
            return container;
        }
    }
}