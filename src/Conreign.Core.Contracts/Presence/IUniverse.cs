using System;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Presence
{
    public interface IUniverse
    {
        Task Disconnect(Guid connectionId);
    }
}