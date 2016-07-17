using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using MediatR;
using Orleans;

namespace Conreign.Core.GameV2
{
    public interface IMessage<out TPayload, out TMeta, out TOut> : IAsyncRequest<TOut>
    {
        TPayload Payload { get; }
        TMeta Meta { get; }
    }

    public interface IEvent<out T> : IAsyncNotification
    {
        T Payload { get; }
        IImmutableSet<string> ConnectionIds { get; }
    }

    public interface IAuthenticationFeature
    {
        string AccessToken { get; }
        IPrincipal Principal { get; }
        // TODO: AuthError
    }

    public interface ISubscriber<T> : IAsyncNotificationHandler<Event<T>>
    {
    }

    public class Metadata : IAuthenticationFeature
    {
        public string ConnectionId { get; set; }

        public string AccessToken { get; set; }

        public IPrincipal Principal { get; set; }

        public Guid? UserId
        {
            get
            {
                var principal = Principal as ClaimsPrincipal;
                var claim = principal?.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    return null;
                }
                return Guid.Parse(claim.Value);
            }
        }
    }

    public static class Message
    {
        public static Message<TIn, TOut> Create<TIn, TOut>(TIn payload, Metadata metadata)
        {
            return new Message<TIn, TOut>(payload, metadata);
        }

        public static Message<TIn, Unit> Create<TIn>(TIn payload, Metadata metadata)
        {
            return new Message<TIn, Unit>(payload, metadata);
        }
    }

    public class Event<T> : IEvent<T>
    {
        public Event(T payload, params string[] connectionIds)
        {
            Payload = payload;
            ConnectionIds = connectionIds.ToImmutableHashSet();
        }

        public IImmutableSet<string> ConnectionIds { get; set; }

        public T Payload { get; }
    }

    public static class Event
    {
        public static Event<T> Create<T>(T payload, params string[] connectionIds)
        {
            return new Event<T>(payload, connectionIds);
        }
    }


    public interface IHandler<TIn, TOut> : IAsyncRequestHandler<Message<TIn, TOut>, TOut>
    {
    }

    public enum PlayerRole
    {
        Owner,
        Member
    }

    public class JoinRoomRequest : IAsyncRequest<JoinRoomResponse>
    {
        public string RoomId { get; set; }
        public string Nickname { get; set; }
    }

    public class JoinRoomResponse
    {
        public bool IsAvailable { get; set; }
        public PlayerRole? Role { get; set; }
    }

    public class GetRoomStateRequest
    {
        public string RoomId { get; set; }
    }

    public class GetRoomStateResponse
    {
        public string RoomId { get; set; }

        public Guid OwnerId { get; set; }

        public List<PlanetState> Planets { get; set; }

        public List<PlayerState> Players { get; set; }

        public GameOptionsState GameOptions { get; set; }
    }

    public class SendChatMessageRequest
    {
        public string ChatId { get; set; }
        public string Text { get; set; }
    }

    public class SendChatNotificationRequest
    {
        public string ChatId { get; set; }
        public object Data { get; set; }
        public HashSet<Guid> UserIds { get; set; }
    }

    public class ChatMessageReceivedEvent
    {
        public Guid SenderId { get; set; }
        public string Nickname { get; set; }
        public string Text { get; set; }
    }

    public class JoinRoomRequestedEvent
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
    }

    public class UserConnectedEvent
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string RoomId { get; set; }
        public string ConnectionId { get; set; }
    }

    public interface IRoom :
        IHandler<JoinRoomRequest, JoinRoomResponse>,
        IHandler<GetRoomStateRequest, GetRoomStateResponse>
    {
    }

    public interface IRoomGrain : IRoom, IGrainWithStringKey
    {
    }

    public class PlayerState
    {
        public Guid UserId { get; set; }

        public bool IsOnline { get; set; }

        public string Nickname { get; set; }

        public string Color { get; set; }
    }

    public class GameOptionsState
    {
        public int MapWidth { get; set; }

        public int MapHeight { get; set; }

        public int NeutralPlanets { get; set; }
    }

    public class PositionState
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public class PlanetState
    {
        public string Name { get; set; }

        public PositionState Position { get; set; }

        public PlayerState Owner { get; set; }
    }

    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T> where TKey : IEquatable<TKey>
    {
        private readonly Func<T, TKey> _keySelector;

        public KeyEqualityComparer(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return _keySelector(x).Equals(_keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return _keySelector(obj).GetHashCode();
        }
    }

    public static class SuperExtensions
    {
        public static Message<TIn, TOut> AsMessage<TIn, TOut>(this TIn payload, Metadata metadata)
        {
            return Message.Create<TIn, TOut>(payload, metadata);
        }

        public static Message<TIn, Unit> AsMessage<TIn>(this TIn payload, Metadata metadata)
        {
            return Message.Create<TIn, Unit>(payload, metadata);
        }
    }

    public class RoomState
    {
        public RoomState(string roomId)
        {
            RoomId = roomId;
            var playerComparer = new KeyEqualityComparer<PlayerState, Guid>(x => x.UserId);
            Players = new HashSet<PlayerState>(playerComparer);
            Candidates = new HashSet<PlayerState>(playerComparer);
            Planets = new List<PlanetState>();
        }

        public string RoomId { get; }

        public bool IsGameStarted { get; set; }

        public HashSet<PlayerState> Players { get; }

        public PlayerState Owner { get; set; }

        public HashSet<PlayerState> Candidates { get; }

        public List<PlanetState> Planets { get; }

        public GameOptionsState GameOptions { get; set; }
    }

    public class Room : IRoom
    {
        private readonly RoomState _state;
        private readonly IMediator _mediator;

        public Room(RoomState state, IMediator mediator)
        {
            _state = state;
            _mediator = mediator;
        }

        public async Task<JoinRoomResponse> Handle(Message<JoinRoomRequest, JoinRoomResponse> message)
        {
            var @event = Event.Create(new UserConnectedEvent
            {
                ConnectionId = message.Meta.ConnectionId,
                RoomId = _state.RoomId,
                Nickname = message.Payload.Nickname,
                UserId = message.Meta.UserId.Value
            });
            var player = _state.Players.FirstOrDefault(x => x.UserId == message.Meta.UserId);
            var isMember = player != null;
            if (_state.IsGameStarted)
            {
                if (isMember)
                {
                    await _mediator.PublishAsync(@event);
                    return new JoinRoomResponse
                    {
                        IsAvailable = true,
                        Role = _state.Owner.UserId == player.UserId
                            ? PlayerRole.Owner
                            : PlayerRole.Member
                    };
                }
                return new JoinRoomResponse
                {
                    IsAvailable = false,
                    Role = null
                };
            }
            
            PlayerRole role;
            if (!isMember)
            {
                player = new PlayerState
                {
                    Nickname = message.Payload.Nickname,
                    Color = "TODO",
                    IsOnline = true,
                    UserId = message.Meta.UserId.Value
                };
            }
            await _mediator.PublishAsync(@event);
            if (_state.Owner == null)
            {
                // TODO: initialize game map, planets, etc...
                _state.Owner = player;
                _state.Players.Add(player);
                role = PlayerRole.Owner;
            }
            else
            {
                // TODO: regenerate map
                var added = _state.Candidates.Add(player);
                role = PlayerRole.Member;
                if (!added)
                {
                    return new JoinRoomResponse
                    {
                        IsAvailable = true,
                        Role = role
                    };
                }
                var payload = new SendChatNotificationRequest
                {
                    ChatId = _state.RoomId,
                    Data = new JoinRoomRequestedEvent
                    {
                        Nickname = player.Nickname,
                        UserId = player.UserId
                    },
                    UserIds = new HashSet<Guid> {_state.Owner.UserId}
                };
                var chatMessage = Message.Create(payload, message.Meta);
                await _mediator.SendAsync(chatMessage);
            }
            return new JoinRoomResponse
            {
                IsAvailable = true,
                Role = role
            };
        }

        public Task<GetRoomStateResponse> Handle(Message<GetRoomStateRequest, GetRoomStateResponse> message)
        {
            var response = new GetRoomStateResponse
            {
                RoomId = _state.RoomId,
                OwnerId = _state.Owner?.UserId ?? Guid.Empty,
                GameOptions = _state.GameOptions,
                Players = _state.Players.ToList(),
                Planets = _state.Planets.ToList()
            };
            return Task.FromResult(response);
        }
    }

    public class RoomGrain : Grain<RoomState>, IRoomGrain
    {
        private readonly IMediator _mediator;
        private Room _room;

        public RoomGrain(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override Task OnActivateAsync()
        {
            State = new RoomState(this.GetPrimaryKeyString());
            _room = new Room(State, _mediator);
            return TaskDone.Done;
        }

        public Task<JoinRoomResponse> Handle(Message<JoinRoomRequest, JoinRoomResponse> message)
        {
            return _room.Handle(message);
        }

        public Task<GetRoomStateResponse> Handle(Message<GetRoomStateRequest, GetRoomStateResponse> message)
        {
            return _room.Handle(message);
        }
    }

    public interface IChat :
        IHandler<SendChatMessageRequest, Unit>,
        IHandler<SendChatNotificationRequest, Unit>,
        ISubscriber<UserConnectedEvent>
    {
    }

    public class UserState
    {
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
        public string Nickname { get; set; }
    }

    public class ChatState
    {
        public ChatState()
        {
            Users = new Dictionary<Guid, UserState>();
        }

        public Dictionary<Guid, UserState> Users { get; }

    }

    public class Chat : IChat
    {
        private readonly ChatState _state;
        private readonly IMediator _mediator;

        public Chat(ChatState state, IMediator mediator)
        {
            _state = state;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(Message<SendChatMessageRequest, Unit> message)
        {
            var sender = _state.Users[message.Meta.UserId.Value];
            var connectionIds = _state.Users.Values
                .Where(x => x.UserId != message.Meta.UserId.Value)
                .Select(x => x.ConnectionId)
                .ToArray();
            var payload = new ChatMessageReceivedEvent
            {
                Nickname = sender.Nickname,
                SenderId = sender.UserId,
                Text = message.Payload.Text
            };
            var @event = Event.Create(payload, connectionIds);
            await _mediator.PublishAsync(@event);
            return Unit.Value;
        }

        public async Task<Unit> Handle(Message<SendChatNotificationRequest, Unit> message)
        {
            var connectionIds = _state.Users.Values
                .Where(x => message.Payload.UserIds.Contains(x.UserId))
                .Select(x => x.ConnectionId)
                .ToArray();
            var @event = Event.Create(message.Payload.Data, connectionIds);
            await _mediator.PublishAsync(@event);
            return Unit.Value;
        }

        public Task Handle(Event<UserConnectedEvent> notification)
        {
            _state.Users[notification.Payload.UserId] = new UserState
            {
                UserId = notification.Payload.UserId,
                ConnectionId = notification.Payload.ConnectionId,
                Nickname = notification.Payload.Nickname
            };
            return Task.FromResult(Unit.Value);
        }
    }
}
