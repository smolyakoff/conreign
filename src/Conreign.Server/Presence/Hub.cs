using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Presence;
using Conreign.Contracts.Presence.Events;
using Conreign.Core.Utility;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Utility;

namespace Conreign.Server.Presence
{
    public class Hub : IHub
    {
        private readonly HubState _state;
        private readonly ITimeProvider _timeProvider;
        private readonly IBroadcastTopic _topic;

        public Hub(HubState state, IBroadcastTopic topic)
            : this(state, topic, new SystemTimeProvider())
        {
        }

        public Hub(HubState state, IBroadcastTopic topic, ITimeProvider timeProvider)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _topic = topic ?? throw new ArgumentNullException(nameof(topic));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        private IEnumerable<Guid> ClientUserIds => _state.Members
            .Select(x => x.Key)
            .Where(IsClientUserId);

        private IEnumerable<Guid> ServerUserIds => _state.Members
            .Select(x => x.Key)
            .Where(IsServerUserId);

        private IEnumerable<HubMemberState> ClientMembers => _state.Members.Values
            .Where(x => x.Type == HubMemberType.Client);

        public Guid? LeaderUserId
        {
            get
            {
                return ClientUserIds
                    .OrderBy(x => _state.JoinOrder.IndexOf(x))
                    .Where(IsOnline)
                    .FirstOrDefault();
            }
        }

        public bool IsEveryoneOffline
        {
            get { return ClientMembers.All(m => m.ConnectionIds.Count == 0); }
        }

        public TimeSpan EveryoneOfflinePeriod
        {
            get
            {
                if (!IsEveryoneOffline)
                {
                    return TimeSpan.Zero;
                }
                var now = _timeProvider.UtcNow;
                var lastDisconnectionTime = ClientMembers
                    .Where(x => x.ConnectionIdsChangedAt != null)
                    .OrderByDescending(x => x.ConnectionIdsChangedAt)
                    .Select(x => x.ConnectionIdsChangedAt)
                    .FirstOrDefault();
                return now - (lastDisconnectionTime ?? _state.CreatedAt);
            }
        }

        public void EnsureServerMembersConnected(HashSet<Guid> userIdsToBeConnected)
        {
            if (userIdsToBeConnected == null)
            {
                throw new ArgumentNullException(nameof(userIdsToBeConnected));
            }
            var existingUserIds = ServerUserIds.ToHashSet();
            var userIdsToConnect = new HashSet<Guid>(userIdsToBeConnected);
            userIdsToConnect.ExceptWith(existingUserIds);
            var userIdsToDisconnect = new HashSet<Guid>(existingUserIds);
            userIdsToDisconnect.ExceptWith(userIdsToBeConnected);
            foreach (var userId in userIdsToConnect)
            {
                _state.Members[userId] = new HubMemberState {Type = HubMemberType.Server};
            }
            foreach (var userId in userIdsToDisconnect)
            {
                _state.Members.Remove(userId);
            }
        }

        public async Task Connect(Guid userId, Guid connectionId)
        {
            var events = WithLeaderCheck(() =>
            {
                var member = GetOrCreateClient(userId);
                var added = member.ConnectionIds.Add(connectionId);
                if (!added)
                {
                    return Enumerable.Empty<IClientEvent>();
                }
                member.ConnectionIdsChangedAt = _timeProvider.UtcNow;
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
                var member = GetOrCreateClient(userId);
                var removed = member.ConnectionIds.Remove(connectionId);
                if (!removed)
                {
                    return Enumerable.Empty<IClientEvent>();
                }
                member.ConnectionIdsChangedAt = _timeProvider.UtcNow;
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
                return Task.CompletedTask;
            }
            var ids = _state.Members.Select(x => x.Key).ToHashSet();
            ids.ExceptWith(userIds);
            return Notify(ids, events);
        }


        public bool IsOnline(Guid userId)
        {
            var exists = _state.Members.TryGetValue(userId, out HubMemberState member);
            return exists &&
                   (member.Type == HubMemberType.Server || member.ConnectionIds.Count > 0);
        }

        public IEnumerable<IClientEvent> GetEvents(Guid userId)
        {
            return _state.Events
                .Where(x => x.Event.IsPublic() || x.Recipients.Contains(userId))
                .Select(x => x.Event);
        }

        private HubMemberState GetOrCreateClient(Guid userId)
        {
            var member = _state.Members.GetOrCreateDefault(userId);
            if (member.Type != HubMemberType.Client)
            {
                throw new InvalidOperationException($"Expected user with id '{userId}' to be a client member.");
            }
            return member;
        }

        private bool IsClientUserId(Guid userId)
        {
            return _state.Members.ContainsKey(userId) && 
                _state.Members[userId].Type == HubMemberType.Client;
        }

        private bool IsServerUserId(Guid userId)
        {
            return _state.Members.ContainsKey(userId) &&
                   _state.Members[userId].Type == HubMemberType.Server;
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
                    _state.Id,
                    userId,
                    PresenceStatus.Online
                );
                yield return statusChanged;
            }
        }

        private IEnumerable<IClientEvent> Leave(Guid userId)
        {
            var statusChanged = new UserStatusChanged(
                _state.Id,
                userId,
                PresenceStatus.Offline
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