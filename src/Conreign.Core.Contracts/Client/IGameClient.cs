using System;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Client
{
    public interface IGameClient
    {
        Task<IClientConnection> Connect(Guid connectionId);
    }
}