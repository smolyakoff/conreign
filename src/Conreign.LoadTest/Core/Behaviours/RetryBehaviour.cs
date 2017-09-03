using System;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Polly;

namespace Conreign.LoadTest.Core.Behaviours
{
    internal sealed class RetryBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly IBotBehaviour<IClientEvent> _next;

        public RetryBehaviour(IBotBehaviour<IClientEvent> next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task Handle(IBotNotification<IClientEvent> notification)
        {
            var context = notification.Context;
            var i = 0;
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3,
                    retry => TimeSpan.FromMilliseconds(100 * retry),
                    (ex, time) =>
                    {
                        context.Logger.Warning(
                            "[Bot:{BotId}:{UserId}]: Retrying handler for {EventType}. Retry number is {Retry}.",
                            context.BotId,
                            context.UserId,
                            notification.Event.GetType().Name,
                            i);
                        i++;
                    });
            return policy.ExecuteAsync(() => _next.Handle(notification));
        }
    }
}