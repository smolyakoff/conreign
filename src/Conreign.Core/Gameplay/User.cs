using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Gameplay
{
    public class User : IUser
    {
        private readonly IUserContext _userContext;
        private readonly IConnectionTracker _connectionTracker;
        private readonly IPlayerFactory _playerFactory;
        private readonly Dictionary<string, IConnectablePlayer> _playersCache;

        public User(IUserContext userContext, IConnectionTracker connectionTracker, IPlayerFactory playerFactory)
        {
            if (connectionTracker == null)
            {
                throw new ArgumentNullException(nameof(connectionTracker));
            }
            if (playerFactory == null)
            {
                throw new ArgumentNullException(nameof(playerFactory));
            }
            _userContext = userContext;
            _connectionTracker = connectionTracker;
            _playerFactory = playerFactory;
            _playersCache = new Dictionary<string, IConnectablePlayer>();
        }

        public async Task<IPlayer> JoinRoom(string roomId)
        {
            var connectionId = _userContext.ConnectionId;
            var player = _playersCache.ContainsKey(roomId)
                ? _playersCache[roomId]
                : await _playerFactory.Create(roomId);
            _playersCache[roomId] = player;
            await _connectionTracker.Track(connectionId, player);
            return player;
        }
    }
}
