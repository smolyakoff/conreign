using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Game.Messages;

namespace Conreign.Core.Contracts.Game
{
    public interface IRoom :
        IHandler<JoinRoomRequest, JoinRoomResponse>,
        IHandler<GetRoomStateRequest, GetRoomStateResponse>
    {
    }
}