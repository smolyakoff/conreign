using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Gameplay
{
    public class User : IUser
    {
        private readonly IUserContext _userContext;
        private readonly ISystemPublisherFactory _systemPublisherFactory;
        private readonly IPlayerFactory _playerFactory;
        private readonly Dictionary<string, IPlayer> _playersCache;
        private IPublisher<IServerEvent> _globalPublisher;

        public User(IUserContext userContext, ISystemPublisherFactory systemPublisherFactory, IPlayerFactory playerFactory)
        {
            if (playerFactory == null)
            {
                throw new ArgumentNullException(nameof(playerFactory));
            }
            _userContext = userContext;
            _systemPublisherFactory = systemPublisherFactory;
            _playerFactory = playerFactory;
            _playersCache = new Dictionary<string, IPlayer>();
        }

        public async Task<IPlayer> JoinRoom(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            _globalPublisher = _globalPublisher ?? await _systemPublisherFactory.CreateSystemPublisher(ServerTopics.Global);
            var connectionId = _userContext.ConnectionId;
            if (_playersCache.ContainsKey(roomId))
            {
                return _playersCache[roomId];
            }
            var player = await _playerFactory.CreatePlayer(roomId);
            _playersCache[roomId] = player;
            var publisher = await _systemPublisherFactory.CreateSystemPublisher(ServerTopics.Player(_userContext.UserId, roomId));
            var @event = new Connected(connectionId, publisher);
            await _globalPublisher.Notify(@event);
            return player;
        }
    }
}
