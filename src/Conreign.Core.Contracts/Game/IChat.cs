using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Game.Events;
using Conreign.Core.Contracts.Game.Messages;
using MediatR;

namespace Conreign.Core.Contracts.Game
{
    public interface IChat :
        IHandler<SendChatMessageRequest, Unit>,
        IHandler<SendChatNotificationRequest, Unit>,
        ISubscriber<UserConnectedEvent>
    {
    }
}