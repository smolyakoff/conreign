using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Presence
{
    public class Universe : IUniverse, IConnectable
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

        public async Task Disconnect(DisconnectCommand command)
        {
            if (!_state.Connections.ContainsKey(command.ConnectionId))
            {
                return;
            }
            var connection = _state.Connections[command.ConnectionId];
            await connection.Disconnect(command);
            _state.Connections.Remove(command.ConnectionId);
        }

        public Task Connect(ConnectCommand command)
        {
            var connection = command.Connection;
            _state.Connections[command.ConnectionId] = connection;
            return Task.CompletedTask;
        }
    }
}