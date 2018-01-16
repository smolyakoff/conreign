using System;
using Conreign.Contracts.Communication;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    public class GameStarted : IServerEvent, IClientEvent
    {
        public GameStarted()
        {
            Timestamp = DateTime.UtcNow;
        }

        public DateTime Timestamp { get; }
    }
}