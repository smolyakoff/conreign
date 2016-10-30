using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.AI
{
    public interface IBotNotification<out TEvent> where TEvent : IClientEvent
    {
        BotContext Context { get; }
        TEvent Event { get; }
    }
}