using System.Threading.Tasks;
using Conreign.Core.Contracts.Game.Actions;

namespace Conreign.Core.Contracts.Game
{
    public interface IPlayerGrain
    {
        Task ChangeSettings(ChangePlayerSettingsAction settings);
    }
}