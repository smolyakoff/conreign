using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Contracts.Game
{
    public interface IGameRoomGrain : IGrainWithStringKey
    {
        Task UpdateGameParameters(UpdateGameParametersAction action);

        Task<IAsyncObservable<object>> Visit(VisitGameRoomAction action);

        Task Leave(LeaveGameRoomAction action);

        Task AcceptPlayer(AcceptPlayerAction action);

        Task StartGame(StartGameAction action);
    }
}