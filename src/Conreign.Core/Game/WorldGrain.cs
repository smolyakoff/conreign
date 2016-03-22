using System.Threading.Tasks;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Actions;
using Conreign.Core.Contracts.Game.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Game
{
    [StatelessWorker]
    public class WorldGrain : IWorldGrain
    {
        private readonly World _world;

        public WorldGrain(World world)
        {
            _world = world;
        }

        public Task<WelcomeMessagePayload> Arrive(ArriveAction action)
        {
            return _world.Handle(action);
        }
    }
}
