using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IHub
    {
        Task Notify(NotifyCommand command);
        Task NotifyEverybody(NotifyEverybodyCommand command);
    }
}