using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class GameStarted : IClientEvent
    {
        [Serializable]
        public class Server : IServerEvent
        {
            public Server(IGame game)
            {
                Game = game;
            }

            public IGame Game { get; }
        }
    }
}