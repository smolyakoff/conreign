using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Auth.Actions
{
    [Action(Internal = true)]
    [Immutable]
    public class AuthenticateAction : IGrainAction<IAuthGrain>, IPayloadContainer<AccessTokenPayload>
    {
        public AuthenticateAction(AccessTokenPayload payload)
        {
            Payload = payload;
        }

        public GrainKey<IAuthGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IAuthGrain>(default(long));

        public AccessTokenPayload Payload { get; }
    }
}