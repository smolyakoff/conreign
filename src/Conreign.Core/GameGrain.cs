using System.Threading.Tasks;
using Conreign.Core.Contracts;
using Orleans;

namespace Conreign.Core
{
    public class GameGrain : Grain<GameState>, IGameGrain
    {
        public async Task<string> SayWelcomeAsync(string user)
        {
            if (!string.IsNullOrEmpty(user))
            {
                State.Name = user;
            }
            await WriteStateAsync();
            return $"Welcome, {State.Name}!";
        }

        public Task ClearAsync()
        {
            return this.ClearStateAsync();
        }
    }

    public class GameState : GrainState
    {
        public GameState()
        {
            Nested = new SomeNestedState();
        }

        public string Name { get; set; }

        public SomeNestedState Nested { get; set; }
    }

    public class SomeNestedState
    {
        public string Address { get; set; }

        public int Year { get; set; }
    }
}
