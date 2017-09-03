using System.Threading.Tasks;

namespace Conreign.Server.Contracts.Communication
{
    public interface ITopicFactory
    {
        Task<ITopic> Create(string id);
    }
}