using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayerFactory
    {
        Task<IConnectablePlayer> Create(string roomId);
    }
}