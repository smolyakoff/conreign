using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Presence
{
    public interface IConnection
    {
        Task Connect(string topicId);
        Task Disconnect();
    }
}
