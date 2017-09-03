using System;
using System.Threading.Tasks;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IGameFactory
    {
        Task<IGame> CreateGame(Guid userId);
    }
}