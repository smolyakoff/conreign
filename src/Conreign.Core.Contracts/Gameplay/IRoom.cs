using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IRoom : IHub, IVisitable
    {
        Task<IRoomState> GetState(Guid userId);
    }
}