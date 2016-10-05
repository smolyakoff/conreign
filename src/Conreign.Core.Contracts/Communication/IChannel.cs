using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Contracts.Communication
{
    public interface IChannel : IObserver, IDisconnectable
    {
    }
}
