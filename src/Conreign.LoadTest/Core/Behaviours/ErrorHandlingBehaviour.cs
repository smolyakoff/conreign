using System;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;

namespace Conreign.LoadTest.Core.Behaviours
{
    internal sealed class ErrorHandlingBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly IBotBehaviour<IClientEvent> _next;

        public ErrorHandlingBehaviour(IBotBehaviour<IClientEvent> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task Handle(IBotNotification<IClientEvent> notification)
        {
            var context = notification.Context;
            try
            {
                await _next.Handle(notification);
            }
            catch (Exception ex)
            {
                context.Logger.Error(ex, "[Bot:{BotId}:{UserId}] Failed to handle {EventType}: {Message}",
                    context.BotId,
                    context.UserId,
                    notification.Event.GetType().Name,
                    ex.Message);
                context.Complete(ex);
            }
        }
    }
}