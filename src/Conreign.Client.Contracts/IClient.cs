using System;
using System.Threading.Tasks;

namespace Conreign.Client.Contracts
{
    public interface IClient
    {
        Task<IClientConnection> Connect(Guid connectionId);
    }
}