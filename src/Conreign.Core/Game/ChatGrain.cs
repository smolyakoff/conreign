using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Game;
using Conreign.Core.Contracts.Game.Events;
using Conreign.Core.Contracts.Game.Messages;
using MediatR;
using Orleans;

namespace Conreign.Core.Game
{
    public class ChatGrain : Grain<ChatState>, IChatGrain
    {
        private readonly IMediator _mediator;
        private Chat _chat;

        public ChatGrain(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override Task OnActivateAsync()
        {
            State = new ChatState(this.GetPrimaryKeyString());
            _chat = new Chat(State, _mediator);
        }

        public Task<Unit> Handle(Message<SendChatMessageRequest, Unit> message)
        {
            throw new System.NotImplementedException();
        }

        public Task<Unit> Handle(Message<SendChatNotificationRequest, Unit> message)
        {
            throw new System.NotImplementedException();
        }

        public Task Handle(Event<UserConnectedEvent> notification)
        {
            throw new System.NotImplementedException();
        }
    }
}
