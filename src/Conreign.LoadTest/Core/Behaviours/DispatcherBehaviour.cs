using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;

namespace Conreign.LoadTest.Core.Behaviours
{
    internal sealed class DispatcherBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly List<IBotBehaviour> _behaviours;

        public DispatcherBehaviour(IEnumerable<IBotBehaviour> behaviours)
        {
            _behaviours = behaviours.ToList();
        }

        public Task Handle(IBotNotification<IClientEvent> notification)
        {
            var eventType = notification.Event.GetType();
            var botNotificationType = typeof(BotNotification<>).MakeGenericType(eventType);
            dynamic internalNotification = Activator.CreateInstance(
                botNotificationType,
                notification.Event,
                notification.Context);
            return HandleInternal(internalNotification);
        }

        private async Task HandleInternal<T>(IBotNotification<T> notification) where T : IClientEvent
        {
            foreach (var behaviour in _behaviours.OfType<IBotBehaviour<T>>())
                await behaviour.Handle(notification);
        }
    }
}