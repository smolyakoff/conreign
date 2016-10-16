using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI
{
    public class Bot : IEventHandler<IClientEvent>
    {
        private readonly BotContext _context;
        private readonly Dictionary<Type, List<IBotBehaviour>> _eventBehaviours;
        private readonly List<IBotBehaviour> _genericBehaviours;

        public static Bot Create(Guid userId, IUser user, IEnumerable<IBotBehaviour> behaviours)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (behaviours == null)
            {
                throw new ArgumentNullException(nameof(behaviours));
            }
            var context = new BotContext(userId, user);
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
                .GroupBy(x => x.Type)
                .ToList();
            var generic = groups
                .Where(x => x.Key == null)
                .SelectMany(x => x)
                .Select(x => x.Behaviour)
                .ToList();
            var eventBehaviours = groups
                .Where(x => x.Key != null)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Behaviour).ToList());
            return new Bot(context, eventBehaviours, generic);
        }

        private Bot(BotContext context, Dictionary<Type, List<IBotBehaviour>> eventBehaviours, List<IBotBehaviour> genericBehaviours)
        {
            _context = context;
            _eventBehaviours = eventBehaviours;
            _genericBehaviours = genericBehaviours;
        }

        public Task Handle(IClientEvent @event)
        {
            if (@event == null)
            {
                return Task.CompletedTask;
            }
            var type = @event.GetType();
            var tasks = _eventBehaviours
                .Where(x => x.Key.IsAssignableFrom(type))
                .SelectMany(x => x.Value)
                .Select(b =>
                {
                    dynamic behaviour = b;
                    return (Task) behaviour.Handle((dynamic)@event, _context);
                })
                .ToList();
            return Task.WhenAll(tasks);
        }

        public Task Start()
        {
            var tasks = _eventBehaviours
                .Values
                .SelectMany(x => x)
                .Concat(_genericBehaviours)
                .OfType<ILifetimeBotBehaviour>()
                .Select(x => x.Start(_context));
            return Task.WhenAll(tasks);
        }
    }
}
