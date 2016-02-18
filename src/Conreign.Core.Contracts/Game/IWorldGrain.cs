using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IWorldGrain : IGrainWithIntegerKey
    {
        Task<PlayerPayload> Arrive();

        Task<GameStatusPayload> CheckGameStatus(CheckGameStatusAction action);

        Task<GameRoomPayload> ReserveGameRoom(ReserveGameRoomAction reservation);
    }
}