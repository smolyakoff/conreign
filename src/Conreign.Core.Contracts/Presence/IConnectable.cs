using System;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Presence
{
    public interface IConnectable
    {
        Task Connect(Guid connectionId);
        Task Disconnect(Guid connectionId);
    }
}