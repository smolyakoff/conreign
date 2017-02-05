using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Contracts.Presence.Events;
using Conreign.Core.Utility;

namespace Conreign.Core.Presence
{
    public class Hub : IHub
    {
        private readonly HubState _state;
        private readonly IUserTopic _topic;

        public Hub(HubState state, IUserTopic topic)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            _state = state;
            _topic = topic;
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

        public async Task Connect(Guid userId, Guid connectionId)
        {
            var member = _state.Members.GetOrCreateDefault(userId);
            member.ConnectionIds.Add(connectionId);
            var isFirstConnection = member.ConnectionIds.Count == 1;
            if (isFirstConnection)
            {
                var events = WithLeaderCheck(() => Join(userId)).Cast<IEvent>().ToArray();
                await this.NotifyEverybodyExcept(userId, events);
            }
        }

        public async Task Disconnect(Guid userId, Guid connectionId)
        {
            if (!_state.Members.ContainsKey(userId))
            {
                return;
            }
            var member = _state.Members.GetOrCreateDefault(userId);
            member.ConnectionIds.Remove(connectionId);
            if (member.ConnectionIds.Count == 0)
            {
                var events = WithLeaderCheck(() => Leave(userId));
                await this.NotifyEverybody(events);
            }
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
            var targetUserIds = _state.Members
                .Select(x => x.Key)
                .Where(userIds.Contains)
                .ToHashSet();
            var targetConnectionIds = _state.Members
                .Where(x => userIds.Contains(x.Key))
                .SelectMany(x => x.Value.ConnectionIds)
                .ToHashSet();
            await _topic.Broadcast(targetUserIds, targetConnectionIds, events);
            var states = events
                .OfType<IClientEvent>()
                .Where(e => e.IsPersistent())
                .Select(x => new EventState
                {
                    Recipients = new HashSet<Guid>(userIds),
                    Event = x
                });
            _state.Events.AddRange(states);
        }

        public async Task NotifyEverybody(params IEvent[] events)
        {
            var ids = _state.Members.Select(x => x.Key).ToHashSet();
            var serverEvents = events.OfType<IServerEvent>().ToArray();
            if (serverEvents.Length > 0)
            {
                await _topic.Send(serverEvents);
            }
            await Notify(ids, events);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events)
        {
            var ids = _state.Members.Select(x => x.Key).ToHashSet();
            ids.ExceptWith(userIds);
            return Notify(ids, events);
        }

        public bool IsOnline(Guid userId)
        {
            return _state.Members.ContainsKey(userId) &&
                   _state.Members[userId].ConnectionIds.Count > 0;
        }

        public IEnumerable<IClientEvent> GetEvents(Guid userId)
        {
            return _state.Events
                .Where(x => x.Recipients.Contains(userId))
                .Select(x => x.Event);
        }

        private IEnumerable<IClientEvent> Join(Guid userId)
        {
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

        private static IEnumerable<IClientEvent> Leave(Guid userId)
        {
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
            var leaderChanged = new LeaderChanged {UserId = currentLeader};
            yield return leaderChanged;
        }
    }
}