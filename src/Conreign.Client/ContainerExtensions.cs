using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Conreign.Client.Handlers;
using Conreign.Client.Handlers.Decorators;
using MediatR;

namespace Conreign.Client
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
            container.RegisterDecorator(typeof(IAsyncRequestHandler<,>), typeof(AuthenticationDecorator<, >), Lifestyle.Singleton);
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
