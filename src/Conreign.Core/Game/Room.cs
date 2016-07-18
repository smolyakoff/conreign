using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Events;
using Conreign.Core.Contracts.Game.Messages;
using MediatR;
using JoinRoomRequestedEvent = Conreign.Core.Contracts.Game.Messages.JoinRoomRequestedEvent;

namespace Conreign.Core.Game
{
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
}