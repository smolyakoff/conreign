using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Presence
{
    public class Universe : IUniverse, IEventHandler<Connected>
    {
        private readonly UniverseState _state;
        private readonly ITopicFactory _topicFactory;

        public Universe(UniverseState state, ITopicFactory topicFactory)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (topicFactory == null)
            {
                throw new ArgumentNullException(nameof(topicFactory));
            }
            _state = state;
            _topicFactory = topicFactory;
        }

        public async Task Disconnect(Guid connectionId)
        {
            if (!_state.Connections.ContainsKey(connectionId))
            {
                return;
            }
            var topic = await _topicFactory.Create(_state.Connections[connectionId]);
            var @event = new Disconnected(connectionId);
            await topic.Send(@event);
            _state.Connections.Remove(connectionId);
        }

        public async Task Handle(Connected @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            var connectionId = @event.ConnectionId;
            var currentTopic = @event.TopicId;
            if (_state.Connections.ContainsKey(connectionId))
            {
                var previousTopic = _state.Connections[connectionId];
                if (previousTopic != currentTopic)
                {
                    var topic = await _topicFactory.Create(previousTopic);
                    await topic.Send(new Disconnected(connectionId));
                }
                return;
            }
            _state.Connections[connectionId] = currentTopic;
        }
    }
}