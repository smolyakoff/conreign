using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class GameStarted : IClientEvent
    {

        [Serializable]
        public class System : ISystemEvent
        {
            public System(IGame game)
            {
                Game = game;
            }

            public IGame Game { get; }
        }
    }
}
