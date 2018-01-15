using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Core.Utility;
using Conreign.Server.Contracts.Communication;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Communication
{
    public class Topic : IBroadcastTopic
    {
        private readonly Dictionary<Guid, IAsyncStream<IServerEvent>> _childrenStreams;
        private readonly Dictionary<Guid, IAsyncStream<IClientEvent>> _clientStreams;
        private readonly string _id;
        private readonly IAsyncStream<IServerEvent> _parentStream;
        private readonly IStreamProvider _provider;

        public Topic(IStreamProvider provider, string id)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            }
            _provider = provider;
            _id = id;
            _parentStream = provider.GetStream<IServerEvent>(default(Guid), id);
            _childrenStreams = new Dictionary<Guid, IAsyncStream<IServerEvent>>();
            _clientStreams = new Dictionary<Guid, IAsyncStream<IClientEvent>>();
        }

        public async Task Send(params IServerEvent[] events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            foreach (var @event in events)
                await _parentStream.OnNextAsync(@event);
        }

        public async Task Broadcast(ISet<Guid> userIds, ISet<Guid> connectionIds, params IEvent[] events)
        {
            var serverEvents = events.OfType<IServerEvent>();
            var clientEvents = events.OfType<IClientEvent>();
            var serverStreams = userIds
                .Select(id => _childrenStreams.GetOrCreateDefault(id, () => CreateUserStream(id)));
            var serverTasks = serverEvents
                .SelectMany(@event => serverStreams.Select(s => s.OnNextAsync(@event)))
                .ToList();
            await Task.WhenAll(serverTasks);
            var clientStreams = connectionIds
                .Select(id => _clientStreams.GetOrCreateDefault(id, () => CreateClientStream(id)));
            var clientTasks = clientEvents
                .SelectMany(@event => clientStreams.Select(s => s.OnNextAsync(@event)))
                .ToList();
            await Task.WhenAll(clientTasks);
        }

        public static Topic Room(IStreamProvider provider, string roomId)
        {
            return new Topic(provider, TopicIds.Room(roomId));
        }

        public static Topic Player(IStreamProvider provider, Guid userId, string roomId)
        {
            return new Topic(provider, TopicIds.Player(userId, roomId));
        }

        public Task<StreamSubscriptionHandle<IServerEvent>> EnsureIsSubscribedOnce<T>(T handler)
            where T : Grain, IEventHandler
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            return handler.EnsureIsSubscribedOnce(_parentStream);
        }

        private IAsyncStream<IServerEvent> CreateUserStream(Guid userId)
        {
            var id = string.Concat(_id, "/", userId);
            return _provider.GetStream<IServerEvent>(Guid.Empty, id);
        }

        private IAsyncStream<IClientEvent> CreateClientStream(Guid connectionId)
        {
            return _provider.GetStream<IClientEvent>(connectionId, StreamConstants.ClientNamespace);
        }
    }
}