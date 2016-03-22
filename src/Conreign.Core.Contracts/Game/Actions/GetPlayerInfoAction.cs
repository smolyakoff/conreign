using System;
using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Routing;
using MediatR;

namespace Conreign.Core.Contracts.Game.Actions
{
    [Action(Internal = true)]
    public class GetPlayerInfoAction : IGrainAction<IPlayerGrain>, IPayloadContainer<KeyPayload<Guid>>, IAsyncRequest<PlayerInfoPayload>
    {
        public GetPlayerInfoAction(Guid playerKey)
        {
            Payload = new KeyPayload<Guid>(playerKey);
        }

        public GrainKey<IPlayerGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IPlayerGrain>(Payload?.Key);

        public KeyPayload<Guid> Payload { get; }
    }
}