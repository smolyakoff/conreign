using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Core.Contracts.Auth.Actions
{
    [Action(Internal = true)]
    public class LoginAnonymousAction : IGrainAction<IAuthGrain>
    {
        public GrainKey<IAuthGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IAuthGrain>(0);
    }
}
