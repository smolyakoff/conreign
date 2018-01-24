using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Core.Battle.AI;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Gameplay
{
    public class BotGrain : Grain<BotState>, IBotGrain
    {
        private Bot _bot;
        private StreamSubscriptionHandle<IServerEvent> _subscription;

        public override async Task OnActivateAsync()
        {
            InitializeState();
            var game = GrainFactory.GetGrain<IGameGrain>(State.RoomId);
            var strategy = new RankingBotBattleStrategy(RankingBotBattleStrategyOptions.Default);
            _bot = new Bot(State, game, strategy);
            var provider = GetStreamProvider(StreamConstants.ProviderName);
            var stream = provider.GetServerStream(TopicIds.Player(State.UserId, State.RoomId));
            _subscription = await this.EnsureIsSubscribedOnce(stream);
        }

        public override Task OnDeactivateAsync()
        {
            return _subscription.UnsubscribeAsync();
        }

        private void InitializeState()
        {
            State.UserId = this.GetPrimaryKey(out string roomId);
            State.RoomId = roomId;
        }

        public Task Handle(GameStarted @event)
        {
            return _bot.Handle(@event);
        }

        public Task Handle(TurnCalculationEnded @event)
        {
            return _bot.Handle(@event);
        }

        public Task Handle(PlayerDead @event)
        {
            return _bot.Handle(@event);
        }

        public Task EnsureIsListening()
        {
            return Task.CompletedTask;
        }
    }
}
