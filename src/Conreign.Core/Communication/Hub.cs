using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Presence;

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

        public Task Join(JoinCommand command)
        {
            _state.Members[command.UserId] = command.Observer;
            return Task.CompletedTask;
        }

        public Task Leave(LeaveCommand command)
        {
            _state.Members.Remove(command.UserId);
            return Task.CompletedTask;
        }

        public async Task Notify(NotifyCommand command)
        {
            var receivers = _state.Members
                .Where(x => command.UserIds.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();
            var tasks = receivers.Select(x => x.Notify(command.Event));
            await Task.WhenAll(tasks);
            _state.Events.Add(new EventState
            {
                Recipients = new HashSet<Guid>(command.UserIds),
                Event = command.Event
            });
        }

        public Task NotifyEverybody(NotifyEverybodyCommand command)
        {
            var ids = new HashSet<Guid>(_state.Members.Select(x => x.Key));
            var notifyCommand = new NotifyCommand
            {
                Event = command.Event,
                UserIds = ids,
            };
            return Notify(notifyCommand);
        }
    }
}