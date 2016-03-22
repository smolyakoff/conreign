using System;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Core.Contracts.Auth;
using Orleans.Streams;

namespace Conreign.Core.Contracts.Game.Streaming
{
    public static class StreamingExtensions
    {
        public const string PlayerStreamNamespace = "Player";

        public const string EventStreamNamespace = "Event";

        public static IAsyncStream<IMetadataContainer<IUserMeta>> GetPlayerStream(this IStreamProvider provider,
            Guid userKey)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return provider.GetStream<IMetadataContainer<IUserMeta>>(userKey, PlayerStreamNamespace);
        }

        public static IAsyncStream<EventEnvelope<object>> GetEventStream(this IStreamProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return provider.GetStream<EventEnvelope<object>>(default(Guid), EventStreamNamespace);
        }
    }
}