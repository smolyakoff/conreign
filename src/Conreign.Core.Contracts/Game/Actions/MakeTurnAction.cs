using Conreign.Core.Contracts.Auth;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class MakeTurnAction : IMetadataContainer<IUserMeta>
    {
        public IUserMeta Meta { get; set; }
    }
}