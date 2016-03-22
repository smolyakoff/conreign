using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Routing;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class LookIntoGameRoomAction : IPayloadContainer<KeyPayload<string>>, IMetadataContainer<Meta>, IGrainAction<IGameRoomGrain>
    {
        public GrainKey<IGameRoomGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IGameRoomGrain>(Payload?.Key);

        public KeyPayload<string> Payload { get; set; }

        public Meta Meta { get; set; }
    }
}