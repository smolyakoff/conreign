using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Server.Contracts.Presence;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IRoom : IConnectable
    {
        Task<IRoomData> GetState(Guid userId);
        Task SendMessage(Guid userId, TextMessageData textMessage);
    }
}