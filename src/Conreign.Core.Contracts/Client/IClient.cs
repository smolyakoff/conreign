using System;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Client
{
    public interface IClient
    {
        Task<IClientConnection> Connect(Guid connectionId);
    }
}