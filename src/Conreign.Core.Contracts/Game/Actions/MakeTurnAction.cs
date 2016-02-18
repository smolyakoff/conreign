using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class MakeTurnAction : IMetadataContainer<IUserMeta>
    {
        public IUserMeta Meta { get; set; }
    }
}