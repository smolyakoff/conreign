using System;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Routing;
using MediatR;
using Orleans;
using Orleans.Streams;

namespace Conreign.Framework.Core
{
    public class EventHandler : IAsyncNotificationHandler<IStreamEvent>
    {
        private readonly Lazy<IStreamProvider> _streamProvider;
        private readonly string _streamProviderName;

        public EventHandler(string streamProviderName)
        {
            if (string.IsNullOrEmpty(streamProviderName))
            {
                throw new ArgumentException("Argument is null or empty", nameof(streamProviderName));
            }
            _streamProviderName = streamProviderName;
            _streamProvider = new Lazy<IStreamProvider>(() => GrainClient.GetStreamProvider(_streamProviderName));
        }

        public async Task Handle(IStreamEvent notification)
        {
            var provider = _streamProvider.Value;
            var stream = provider.GetStream<IStreamEvent>(notification.StreamKey.Id, notification.StreamKey.Namespace);
            await stream.OnNextAsync(notification);
        }
    }
}