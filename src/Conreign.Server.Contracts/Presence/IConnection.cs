using System.Threading.Tasks;

namespace Conreign.Server.Contracts.Presence
{
    public interface IConnection
    {
        Task Connect(string topicId);
        Task Disconnect();
    }
}