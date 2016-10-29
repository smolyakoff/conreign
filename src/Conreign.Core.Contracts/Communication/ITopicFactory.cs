using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface ITopicFactory
    {
        Task<ITopic> Create(string id);
    }
}