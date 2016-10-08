using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Presence
{
    public class Universe : IUniverse, IConnectionTracker
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
            var connection = _state.Connections[connectionId];
            await connection.Disconnect(connectionId);
            _state.Connections.Remove(connectionId);
        }

        public async Task Track(Guid connectionId, IConnectable connectable)
        {
            if (_state.Connections.ContainsKey(connectionId))
            {
                var previousConnection = _state.Connections[connectionId];
                if (previousConnection != connectable)
                {
                    await previousConnection.Disconnect(connectionId);
                }
                return;
            }
            await connectable.Connect(connectionId);
            _state.Connections[connectionId] = connectable;
        }
    }
}