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

        public Universe(UniverseState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _state = state;
        }

        public async Task Disconnect(Guid connectionId)
        {
            if (!_state.Connections.ContainsKey(connectionId))
            {
                return;
            }
            var bus = _state.Connections[connectionId];
            var @event = new Disconnected(connectionId);
            await bus.Notify(@event);
            _state.Connections.Remove(connectionId);
        }

        public async Task Handle(Connected @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            var connectionId = @event.ConnectionId;
            var connection = @event.Connection;
            if (_state.Connections.ContainsKey(connectionId))
            {
                var previousConnection = _state.Connections[connectionId];
                if (previousConnection != connection)
                {
                    await previousConnection.Notify(new Disconnected(connectionId));
                }
                return;
            }
            await connection.Notify(new Connected(connectionId, connection));
            _state.Connections[connectionId] = connection;
        }
    }
}