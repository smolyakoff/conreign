using System;
using Conreign.Core.Contracts.Abstractions;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Game.Actions
{
    [Action(Internal = true)]
    [Immutable]
    public class GetPlayerSettingsAction : IGrainAction<IPlayerMembershipGrain>, IPayloadContainer<Guid>
    {
        public GetPlayerSettingsAction(Guid playerKey)
        {
            Payload = playerKey;
        }

        public GrainKey<IPlayerMembershipGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IPlayerMembershipGrain>(default(long));

        public Guid Payload { get; }
    }
}