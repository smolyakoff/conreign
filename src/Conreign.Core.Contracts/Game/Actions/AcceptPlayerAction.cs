using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Data;
using Conreign.Core.Contracts.Game.Data;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class AcceptPlayerAction : IMetadataContainer<IUserMeta>, IPayloadContainer<PlayerReferencePayload>
    {
        public IUserMeta Meta { get; set; }

        public PlayerReferencePayload Payload { get; set; }
    }
}