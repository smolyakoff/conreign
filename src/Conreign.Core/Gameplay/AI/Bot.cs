using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Gameplay.AI.Behaviours;

namespace Conreign.Core.Gameplay.AI
{
    public class Bot : IEventHandler<IClientEvent>
    {
        private readonly BotContext _context;
        private readonly Dictionary<Type, List<IBotBehaviour>> _behaviours;
        private ActionBlock<IClientEvent> _processor;

        public static Bot Create(string readableId, Guid userId, IUser user, IEnumerable<IBotBehaviour> behaviours)
        {
            if (string.IsNullOrEmpty(readableId))
            {
                throw new ArgumentException("Readable id cannot be null or empty.", nameof(readableId));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (behaviours == null)
            {
                throw new ArgumentNullException(nameof(behaviours));
            }
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
            var behaviors = groups
                .ToDictionary(x => x.Key, x => x.Select(y => y.Behaviour).ToList());
            return new Bot(readableId, userId, user, behaviors);
        }

        private Bot(string readableId, Guid userId, IUser user, Dictionary<Type, List<IBotBehaviour>> behaviours)
        {
            _processor = _processor = new ActionBlock<IClientEvent>(Process);
            _context = new BotContext(readableId, userId, user, _processor.Complete);
            _behaviours = behaviours;
        }

        public Task Completion => _processor.Completion;

        public Task Handle(IClientEvent @event)
        {
            return @event == null 
                ? Task.CompletedTask 
                : _processor.SendAsync(@event);
        }

        public Task Start()
        {
            if (_processor.Completion.IsCompleted)
            {
                _processor = new ActionBlock<IClientEvent>(Process);
            }
            return Handle(new BotStarted());
        }

        public Task Stop()
        {
            return Handle(new BotStopped());
        }

        private async Task Process(IClientEvent @event)
        {
            var type = @event.GetType();
            var tasks = _behaviours
                .Where(x => x.Key.IsAssignableFrom(type))
                .SelectMany(x => x.Value)
                .Select(b =>
                {
                    dynamic behaviour = b;
                    return (Task)behaviour.Handle((dynamic)@event, _context);
                })
                .ToList();
            foreach (var task in tasks)
            {
                await task;
            }
        }
    }
}
