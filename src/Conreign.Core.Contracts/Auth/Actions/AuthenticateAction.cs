using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth.Data;

namespace Conreign.Core.Contracts.Auth.Actions
{
    public class AuthenticateAction : IGrainAction<IAuthGrain>, IMetadataContainer<IAuthMeta>
    {
        public GrainKey<IAuthGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IAuthGrain>(default(long));

        public IAuthMeta Meta { get; set; }
    }
}