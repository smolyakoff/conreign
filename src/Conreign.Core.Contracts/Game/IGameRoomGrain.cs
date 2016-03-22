using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IGameRoomGrain : IGrainWithStringKey
    {
        Task<GameRoomPayload> LookInto(LookIntoGameRoomAction action);

        Task<GameRoomStatusPayload> Enter(EnterGameRoomAction action);

        Task Leave(LeaveGameRoomAction action);
    }
}