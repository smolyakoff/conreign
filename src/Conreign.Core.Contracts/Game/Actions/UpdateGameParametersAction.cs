using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Data;
using Conreign.Core.Contracts.Game.Data;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class UpdateGameParametersAction : IGrainAction<IGameRoomGrain>, IPayloadContainer<GameParametersPayload>,
        IMetadataContainer<IUserMeta>
    {
        public GrainKey<IGameRoomGrain> GrainKey => GrainKeyFactory.KeyOrNullFor<IGameRoomGrain>(Payload?.GameKey);

        public IUserMeta Meta { get; set; }

        public GameParametersPayload Payload { get; set; }
    }
}