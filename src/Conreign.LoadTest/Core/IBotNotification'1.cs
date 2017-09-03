using Conreign.Contracts.Communication;

namespace Conreign.LoadTest.Core
{
    public interface IBotNotification<out TEvent> where TEvent : IClientEvent
    {
        BotContext Context { get; }
        TEvent Event { get; }
    }
}