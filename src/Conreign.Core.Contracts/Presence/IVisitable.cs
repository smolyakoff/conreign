using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Presence
{
    public interface IVisitable
    {
        Task Join(JoinCommand command);
        Task Leave(LeaveCommand command);
    }
}
