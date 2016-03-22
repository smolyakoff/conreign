using Conreign.Core.Contracts.Abstractions.Data;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Routing;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class EnterGameRoomAction : IPayloadContainer<KeyPayload<string>>, IGrainAction<IGameRoomGrain>, IMetadataContainer<Meta>
    {
        public GrainKey<IGameRoomGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IGameRoomGrain>(Payload?.Key);

        public Meta Meta { get; set; }

        public KeyPayload<string> Payload { get; set; }
    }
}