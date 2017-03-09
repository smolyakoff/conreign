using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class GameStarted : IClientEvent
    {
        public GameStarted()
        {
            Timestamp = DateTime.UtcNow;
        }

        [Serializable]
        public class Server : IServerEvent
        {
            public Server(IGame game)
            {
                Game = game;
            }

            public IGame Game { get; }
        }

        public DateTime Timestamp { get; }
    }
}