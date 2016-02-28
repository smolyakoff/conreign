using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;

namespace Conreign.Core.Contracts.Game
{
    public interface IPlayerMembersipProvider
    {
        Task<PlayerSettingsPayload> GetPlayerSettings(GetPlayerSettingsAction action);

        Task Connect(ConnectAction action);

        Task Disconnect(DisconnectAction action);
    }
}
