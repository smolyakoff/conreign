using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Contracts.Presence;
using Orleans;

namespace Conreign.Server.Presence
{
    public class ConnectionGrain : Grain<ConnectionState>, IConnectionGrain
    {
        private Guid ConnectionId => this.GetPrimaryKey();

        public async Task<IPlayerClient> Bind(Guid userId, string roomId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException(nameof(userId));
            }
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }
            if (State.RoomId != roomId || State.UserId != userId)
            {
                await EnsureIsDisconnected();
            }
            var player = GrainFactory.GetGrain<IPlayerGrain>(userId, roomId, null);
            await player.Connect(ConnectionId);
            State.RoomId = roomId;
            State.UserId = userId;
            return player;
        }

        public async Task Disconnect()
        {
            await EnsureIsDisconnected();
            DeactivateOnIdle();
        }

        private async Task EnsureIsDisconnected()
        {
            if (State.RoomId == null)
            {
                return;
            }
            var player = GrainFactory.GetGrain<IPlayerGrain>(State.UserId, State.RoomId, null);
            await player.Disconnect(ConnectionId);
            State.UserId = Guid.Empty;
            State.RoomId = null;
        }
    }
}