using System;
using System.Threading.Tasks;

namespace Conreign.Core.Client
{
    public interface IGameClient
    {
        Task<IGameConnection> Connect(Guid connectionId);
    }
}