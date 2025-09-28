using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IRoom : IHub
{
    Task<IRoomData> GetState(Guid userId);
}