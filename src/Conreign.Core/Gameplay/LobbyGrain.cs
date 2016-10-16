using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Utility;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Gameplay
{

    public static class Extensions
    {
        public static async Task<StreamSubscriptionHandle<TEvent>> EnsureIsSubscribedOnce<T, TEvent>(this T handler, IAsyncStream<TEvent> stream)
            where T : Grain, IEventHandler 
            where TEvent : IEvent
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            var eventTypes = handler
                .GetType()
                .GetInterfaces()
                .Where(x => typeof(IEventHandler).IsAssignableFrom(x) && x.IsGenericType)
                .Select(x => x.GetGenericArguments()[0])
                .Where(x => typeof(IEvent).IsAssignableFrom(x))
                .ToList();
            if (eventTypes.Count == 0)
            {
                // TODO : validation
                throw new ArgumentException();
            }
            var handles = await stream.GetAllSubscriptionHandles();
            if (handles.Count > 0)
            {
                await handles[0].ResumeAsync(new ObserverTest<T,TEvent>(handler, eventTypes));
                return handles[0];
            }
            try
            {
                var handle =
                    await stream.SubscribeAsync(new ObserverTest<T, TEvent>(handler, eventTypes));
                return handle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private class ObserverTest<T, TEvent> : IAsyncObserver<TEvent> where T : Grain, IEventHandler where TEvent : IEvent
        {
            private readonly T _grain;
            private readonly List<Type> _types;

            public ObserverTest(T grain, List<Type> types)
            {
                _grain = grain;
                _types = types;
            }

            public Task OnNextAsync(TEvent item, StreamSequenceToken token = null)
            {
                if (!_types.Any(x => x.IsInstanceOfType(item)))
                {
                    return Task.CompletedTask;
                }
                dynamic h = _grain;
                h.Handle((dynamic)item);
                return Task.CompletedTask;
            }

            public Task OnCompletedAsync()
            {
                return Task.CompletedTask;
            }

            public Task OnErrorAsync(Exception ex)
            {
                return Task.CompletedTask;
            }
        }
    }

    

    public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
    {
        private Lobby _lobby;
        private StreamSubscriptionHandle<IServerEvent> _subscription;

        public override async Task OnActivateAsync()
        {
            var stream = GetStreamProvider(StreamConstants.ClientStreamProviderName)
                .GetStream<IServerEvent>(Guid.Empty, ServerTopics.Room(this.GetPrimaryKeyString()));
            InitializeState(stream);
            _lobby = new Lobby(State, this);
            _subscription = await this.EnsureIsSubscribedOnce(stream);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync();
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            Console.WriteLine("Lobby grain get state");
            var x = 1;
            return _lobby.GetState(userId);
        }

        public Task Notify(ISet<Guid> users, params IEvent[] @event)
        {
            return _lobby.Notify(users, @event);
        }

        public Task NotifyEverybody(params IEvent[] @event)
        {
            return _lobby.NotifyEverybody(@event);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events)
        {
            return _lobby.NotifyEverybodyExcept(users, events);
        }

        public Task Join(Guid userId, IPublisher<IEvent> publisher)
        {
            return _lobby.Join(userId, publisher);
        }

        public Task Leave(Guid userId)
        {
            return _lobby.Leave(userId);
        }

        public Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            return _lobby.UpdateGameOptions(userId, options);
        }

        public Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            return _lobby.UpdatePlayerOptions(userId, options);
        }

        public Task GenerateMap(Guid userId)
        {
            return _lobby.GenerateMap(userId);
        }

        public async Task<IGame> StartGame(Guid userId)
        {
            var game = await _lobby.StartGame(userId);
            // DeactivateOnIdle();
            return game;
        }

        private void InitializeState(IAsyncStream<IServerEvent> stream)
        {
            State.RoomId = this.GetPrimaryKeyString();
            State.Hub.Self = new Publisher<IServerEvent>(stream);
        }

        public async Task<IGame> CreateGame()
        {
            var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
            var command = new InitialGameData(
                map: State.MapEditor.Map,
                players: State.Players,
                hub: State.Hub.Self,
                hubMembers: State.Hub.Members,
                hubJoinOrder: State.Hub.JoinOrder
            );
            await game.Initialize(command);
            return game;
        }

        public Task Handle(GameEnded @event)
        {
            return _lobby.Handle(@event);
        }
    }
}
