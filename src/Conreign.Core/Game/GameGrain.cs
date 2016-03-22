using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Orleans;

namespace Conreign.Core.Game
{
    public class GameGrain : Grain, IGameGrain
    {
    }
}