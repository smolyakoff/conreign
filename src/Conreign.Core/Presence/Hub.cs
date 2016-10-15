using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Contracts.Presence.Events;

namespace Conreign.Core.Presence
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

        public async Task Join(Guid userId, IPublisher<IEvent> publisher)
        {
            var events = WithLeaderCheck(() => JoinInternal(userId, publisher));
            await this.NotifyEverybody(events);
        }

        public async Task Leave(Guid userId)
        {
            var events = WithLeaderCheck(() => LeaveInternal(userId));
            await this.NotifyEverybody(events);
        }

        public async Task Notify(ISet<Guid> userIds, params IEvent[] events)
        {
            if (userIds == null)
            {
                throw new ArgumentNullException(nameof(userIds));
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            var receivers = _state.Members
                .Where(x => userIds.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();
            var tasks = receivers.Select(x => x.Notify(events)).ToList();
            await Task.WhenAll(tasks);
            var states = events
                .OfType<IClientEvent>()
                .Select(x => new EventState
                {
                    Recipients = new HashSet<Guid>(userIds),
                    Event = x
                });
            _state.Events.AddRange(states);
        }

        public async Task NotifyEverybody(params IEvent[] events)
        {
            var ids = new HashSet<Guid>(_state.Members.Select(x => x.Key));
            var serverEvents = events.OfType<IServerEvent>().ToArray();
            if (serverEvents.Length > 0)
            {
                await _state.Self.Notify(serverEvents);
            }
            await Notify(ids, events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events)
        {
            var ids = new HashSet<Guid>(_state.Members.Select(x => x.Key));
            ids.ExceptWith(users);
            return Notify(ids, events);
        }

        public bool HasMemberOnline(Guid userId)
        {
            return _state.Members.ContainsKey(userId);
        }

        public IEnumerable<IClientEvent> GetEvents(Guid userId)
        {
            return _state.Events
                .Where(x => x.Recipients.Contains(userId))
                .Select(x => x.Event);
        }

        public Guid? LeaderUserId
        {
            get
            {
                return _state.Members
                    .Select(x => x.Key)
                    .OrderBy(x => _state.JoinOrder.IndexOf(x))
                    .FirstOrDefault();
            }
        }

        private IEnumerable<IClientEvent> JoinInternal(Guid userId, IPublisher<IEvent> publisher)
        {
            _state.Members[userId] = publisher;
            if (!_state.JoinOrder.Contains(userId))
            {
                _state.JoinOrder.Add(userId);
            }
            var statusChanged = new UserStatusChanged
            {
                Status = PresenceStatus.Online,
                UserId = userId
            };
            yield return statusChanged;

        }

        private IEnumerable<IClientEvent> LeaveInternal(Guid userId)
        {
            var removed = _state.Members.Remove(userId);
            if (!removed)
            {
                yield break;
            }
            var @event = new UserStatusChanged
            {
                Status = PresenceStatus.Offline,
                UserId = userId
            };
            yield return @event;
        }

        private IEnumerable<IClientEvent> WithLeaderCheck(Func<IEnumerable<IClientEvent>> func)
        {
            var previousLeader = LeaderUserId;
            foreach (var @event in func())
            {
                yield return @event;
            }
            var currentLeader = LeaderUserId;
            if (previousLeader == currentLeader)
            {
                yield break;
            }
            var leaderChanged = new LeaderChanged { UserId = currentLeader };
            yield return leaderChanged;
        }
    }
}