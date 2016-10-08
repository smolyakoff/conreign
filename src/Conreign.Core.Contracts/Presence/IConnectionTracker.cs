using System;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Presence
{
    public interface IConnectionTracker
    {
        Task Track(Guid connectionId, IConnectable connectable);
    }
}