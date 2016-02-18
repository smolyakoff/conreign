using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IGameChatGrain : IGrainWithStringKey
    {
        Task SendChatMessage(SendChatMessageAction action);
    }
}