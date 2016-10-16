using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IPingable
    {
        Task Ping();
    }
}
