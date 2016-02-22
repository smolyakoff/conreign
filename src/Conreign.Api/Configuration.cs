using System;
using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using Conreign.Api.Framework;
using Conreign.Api.Framework.Owin;
using MediatR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Conreign.Api
{
    public static class Configuration
    {
        private static readonly Lazy<IContainer> ContainerLazy = new Lazy<IContainer>(ConfigureContainer);

        public static IContainer Container => ContainerLazy.Value;

        public static HubConfiguration CreateHubConfiguration()
        {
            var config = new HubConfiguration {Resolver = new AutofacDependencyResolver(Container)};
            return config;
        }

        public static FrameworkOptions CreateFrameworkOptions()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            // ReSharper disable once UseObjectOrCollectionInitializer
            var options = new FrameworkOptions
            {
                ObjectFactory = Container.Resolve<SingleInstanceFactory>(),
                SerializerSettings = serializerSettings
            };
#if DEBUG
            options.Debug = true;
#endif
            return options;
        }

        private static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ApiModule());
            return builder.Build();
        }
    }
}
