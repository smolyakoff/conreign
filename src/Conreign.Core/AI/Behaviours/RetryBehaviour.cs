using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Polly;

namespace Conreign.Core.AI.Behaviours
{
    internal sealed class RetryBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly IBotBehaviour<IClientEvent> _next;

        public RetryBehaviour(IBotBehaviour<IClientEvent> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public Task Handle(IBotNotification<IClientEvent> notification)
        {
            var context = notification.Context;
            var i = 0;
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, 
                    retry => TimeSpan.FromSeconds(Math.Pow(2, retry)), 
                    (ex, time) =>
                    {
                        context.Logger.Warning(
                            "[{ReadableId}-{UserId}]: Retrying event handler {EventType} for the {Retry} time.",
                            context.ReadableId,
                            context.UserId,
                            notification.Event.GetType(),
                            i);
                        i++;
                    });
            return policy.ExecuteAsync(() => _next.Handle(notification));
        }
    }
}