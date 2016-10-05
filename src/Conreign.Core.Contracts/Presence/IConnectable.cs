using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Presence
{
    public interface IConnectable
    {
        Task Connect(ConnectCommand command);
    }
}