using System;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Routing;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Game.Actions
{
    [Immutable]
    public class DisconnectAction : IMetadataContainer<Meta>, IPayloadContainer<ConnectionPayload>,
        IGrainAction<IPlayerGrain>
    {
        public DisconnectAction(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }
            Payload = new ConnectionPayload(connectionId);
        }

        public GrainKey<IPlayerGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IPlayerGrain>(Meta?.User?.UserKey);

        public Meta Meta { get; set; }

        public ConnectionPayload Payload { get; }
    }
}