using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Presence
{
    public interface IDisconnectable
    {
        Task Disconnect(DisconnectCommand command);
    }
}