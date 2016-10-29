using System;
using Microsoft.AspNet.SignalR.Hubs;
using SimpleInjector;

namespace Conreign.Api.Infrastructure
{
    public class SimpleInjectorHubActivator : IHubActivator
    {
        private readonly Container _container;

        public SimpleInjectorHubActivator(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return _container.GetInstance(descriptor.HubType) as IHub;
        }
    }
}