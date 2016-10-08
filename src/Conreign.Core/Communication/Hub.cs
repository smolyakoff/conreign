using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Communication
{
    public class Hub : IHub, IVisitable
    {
        private readonly HubState _state;

        public Hub(HubState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _state = state;
        }

        public async Task Join(Guid userId, IObserver observer)
        {
            _state.Members[userId] = observer;
            await NotifyEverybody(new UserStatusChanged
            {
                IsOnline = true,
                UserId = userId
            });
        }

        public Task Leave(Guid userId)
        {
            _state.Members.Remove(userId);
            return Task.CompletedTask;
        }

        public async Task Notify(object @event, ISet<Guid> userIds)
        {
            var receivers = _state.Members
                .Where(x => userIds.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();
            var tasks = receivers.Select(x => x.Notify(@event)).ToList();
            await Task.WhenAll(tasks);
            _state.Events.Add(new EventState
            {
                Recipients = new HashSet<Guid>(userIds),
                Event = @event
            });
        }

        public Task NotifyEverybody(object @event)
        {
            var ids = new HashSet<Guid>(_state.Members.Select(x => x.Key));
            return Notify(@event, ids);
        }
    }
}