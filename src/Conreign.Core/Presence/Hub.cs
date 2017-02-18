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
                    .Where(IsOnline)
                    .FirstOrDefault();
            }
        }

        public async Task Connect(Guid userId, Guid connectionId)
        {
            var events = WithLeaderCheck(() =>
            {
                var member = _state.Members.GetOrCreateDefault(userId);
                member.ConnectionIds.Add(connectionId);
                var isFirstConnection = member.ConnectionIds.Count == 1;
                return isFirstConnection ? Join(userId) : Enumerable.Empty<IClientEvent>();
            });
            await this.NotifyEverybodyExcept(userId, events.Cast<IEvent>().ToArray());
        }

        public async Task Disconnect(Guid userId, Guid connectionId)
        {
            if (!_state.Members.ContainsKey(userId))
            {
                return;
            }
            var events = WithLeaderCheck(() =>
            {
                var member = _state.Members.GetOrCreateDefault(userId);
                member.ConnectionIds.Remove(connectionId);
                var noConnections = member.ConnectionIds.Count == 0;
                return noConnections ? Leave(userId) : Enumerable.Empty<IClientEvent>();
            });
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
            if (events.Length == 0)
            {
                return;
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
            if (events.Length == 0)
            {
                return;
            }
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
            if (events.Length == 0)
            {
                return TaskCompleted.Completed;
            }
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
            else
            {
                var statusChanged = new UserStatusChanged(
                    hubId: _state.Id,
                    userId: userId,
                    status: PresenceStatus.Online
                    );
                yield return statusChanged;
            }
        }

        private IEnumerable<IClientEvent> Leave(Guid userId)
        {
            var statusChanged = new UserStatusChanged(
                hubId: _state.Id,
                userId: userId,
                status: PresenceStatus.Offline
            );
            yield return statusChanged;
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
            var leaderChanged = new LeaderChanged(_state.Id, currentLeader);
            yield return leaderChanged;
        }
    }
}