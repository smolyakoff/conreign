using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Server.Contracts.Communication;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IRoom : IHub
    {
        Task<IRoomData> GetState(Guid userId);
    }
}