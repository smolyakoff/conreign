using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Events;
using Conreign.Core.Contracts.Game.Messages;
using MediatR;

namespace Conreign.Core.Game
{
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
            var payload = new ChatMessageSentEvent
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
