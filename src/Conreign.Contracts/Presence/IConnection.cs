using System.Threading.Tasks;

namespace Conreign.Contracts.Presence
{
    public interface IConnection
    {
        Task Connect(string topicId);
        Task Disconnect();
    }
}