using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Shared.Communication;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Core.Communication;

internal class EventObserver<T, TEvent> : IAsyncObserver<TEvent> where T : Grain, IEventHandler
    where TEvent : IEvent
{
    private readonly T _grain;
    private readonly List<Type> _types;

    public EventObserver(T grain, List<Type> types)
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
        return (Task)h.Handle((dynamic)item);
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