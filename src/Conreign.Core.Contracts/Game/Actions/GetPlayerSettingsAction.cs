using System;
using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Core.Contracts.Game.Data;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Routing;
using MediatR;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Game.Actions
{
    [Immutable]
    public class GetPlayerSettingsAction : IGrainAction<IPlayerGrain>, IPayloadContainer<KeyPayload<Guid>>, IMetadataContainer<Meta>, IAsyncRequest<PlayerSettingsPayload>
    {
        public GetPlayerSettingsAction(Guid playerKey)
        {
            Payload = new KeyPayload<Guid>(playerKey);
        }

        public GrainKey<IPlayerGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IPlayerGrain>(Payload?.Key);

        public Meta Meta { get; set; }

        public KeyPayload<Guid> Payload { get; set; }
    }
}