using System;
using Conreign.Contracts.Communication;

namespace Conreign.Server.Contracts.Gameplay
{
    [Serializable]
    public class GameStartedServer : IServerEvent
    {
        public GameStartedServer(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}