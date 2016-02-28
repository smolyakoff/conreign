using System;
using Conreign.Core.Contracts.Abstractions;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Auth.Actions
{
    [Action(Internal = true)]
    [Immutable]
    public class LoginAnonymousAction : IGrainAction<IAuthGrain>, IPayloadContainer<Guid>
    {
        public LoginAnonymousAction(Guid playerKey)
        {
            Payload = playerKey;
        }

        public GrainKey<IAuthGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IAuthGrain>(0);

        public Guid Payload { get; }
    }
}
