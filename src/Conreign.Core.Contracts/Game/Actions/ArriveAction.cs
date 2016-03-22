using Conreign.Core.Contracts.Game.Data;
using Conreign.Framework.Contracts.Core.Data;
using MediatR;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class ArriveAction : IMetadataContainer<Meta>, IAsyncRequest<WelcomeMessagePayload>
    {
        public Meta Meta { get; set; }
    }
}