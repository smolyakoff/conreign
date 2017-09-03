using System;
using System.Threading.Tasks;

namespace Conreign.Contracts.Presence
{
    public interface IConnectable
    {
        Task Connect(Guid userId, Guid connectionId);
        Task Disconnect(Guid userId, Guid connectionId);
    }
}