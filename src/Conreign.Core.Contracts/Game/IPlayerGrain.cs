using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;
using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task Connect(ConnectAction action);

        Task Disconnect(DisconnectAction action);

        Task<PlayerInfoPayload> GetInfo(GetPlayerInfoAction action);

        Task<PlayerSettingsPayload> GetSettings(GetPlayerSettingsAction settings);

        Task<PlayerSettingsPayload> SaveSettings(SavePlayerSettingsAction settings);
    }
}