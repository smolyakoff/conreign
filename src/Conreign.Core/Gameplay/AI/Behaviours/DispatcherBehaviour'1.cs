using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    internal sealed class DispatcherBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly Dictionary<Type, List<IBotBehaviour>> _behaviours;

        public DispatcherBehaviour(IEnumerable<IBotBehaviour> behaviours)
        {
            // TODO: beautify code
            var groups = behaviours
                .ToList()
                .SelectMany(b =>
                {
                    var types = b
                        .GetType()
                        .GetInterfaces()
                        .Select(t =>
                        {
                            var type = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IBotBehaviour<>)
                                ? t.GetGenericArguments()[0]
                                : null;
                            return new
                            {
                                Type = type,
                                Behaviour = b
                            };
                        });
                    return types;
                })
                .Where(x => x.Type != null)
                .GroupBy(x => x.Type)
                .ToList();
            _behaviours = groups.ToDictionary(x => x.Key, x => x.Select(y => y.Behaviour).ToList());
        }

        public async Task Handle(IBotNotification<IClientEvent> notification)
        {
            var @event = notification.Event;
            var context = notification.Context;
            var type = @event.GetType();
            var tasks = _behaviours
                .Where(x => x.Key.IsAssignableFrom(type))
                .SelectMany(x => x.Value)
                .Select(b =>
                {
                    dynamic behaviour = b;
                    var notificationType = typeof(BotNotification<>).MakeGenericType(@event.GetType());
                    dynamic concreteNotification = Activator.CreateInstance(notificationType, @event, context);
                    return (Task)behaviour.Handle(concreteNotification);
                })
                .ToList();
            foreach (var task in tasks)
            {
                await task;
            }
        }
    }
}
