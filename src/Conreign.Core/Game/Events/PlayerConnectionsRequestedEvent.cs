using System;
using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Core.Contracts.Auth;
using Orleans.Concurrency;

namespace Conreign.Core.Game.Events
{
    [Immutable]
    internal class PlayerConnectionsRequestedEvent : IMetadataContainer<IUserMeta>
    {
        public PlayerConnectionsRequestedEvent(IUserMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }
            Meta = meta;
        }

        public IUserMeta Meta { get; }
    }
}