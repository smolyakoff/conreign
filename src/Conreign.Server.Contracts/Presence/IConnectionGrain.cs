using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Orleans;

namespace Conreign.Server.Contracts.Presence
{
    public interface IConnectionGrain : IGrainWithGuidKey
    {
        Task<IPlayerClient> Bind(Guid userId, string roomId);
        Task Disconnect();
    }
}