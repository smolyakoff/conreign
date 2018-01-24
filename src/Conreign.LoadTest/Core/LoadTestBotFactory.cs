using System;
using System.Collections.Generic;
using Conreign.Client.Contracts;
using Conreign.LoadTest.Core.Behaviours;

namespace Conreign.LoadTest.Core
{
    public class LoadTestBotFactory : IBotFactory
    {
        private readonly NaiveBotBattleStrategy _battleStrategy;
        private readonly LoadTestBotOptions _options;
        private int _i;

        public LoadTestBotFactory(LoadTestBotOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _battleStrategy = new NaiveBotBattleStrategy();
        }

        public Bot Create(IClientConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (!CanCreate)
            {
                var total = _options.BotsPerRoomCount * _options.RoomsCount;
                throw new InvalidOperationException($"Expected to create maximum of {total} bots.");
            }
            var roomIndex = _i / _options.BotsPerRoomCount;
            var room = $"{_options.RoomPrefix}conreign-load-test-{roomIndex}";
            var k = _i % _options.BotsPerRoomCount;
            var isLeader = _i % _options.BotsPerRoomCount == 0;
            var name = isLeader ? "leader" : $"bot-{k}";
            var id = $"{room}_{name}";
            var behaviours = new List<IBotBehaviour>
            {
                new LoginBehaviour(),
                new JoinRoomBehaviour(room, isLeader ? TimeSpan.Zero : _options.JoinRoomDelay),
                new BattleBehaviour(_battleStrategy),
                new StopOnGameEndBehaviour()
            };
            if (isLeader)
            {
                behaviours.Add(new StartGameBehaviour(_options.BotsPerRoomCount));
            }
            var bot = new Bot(id, connection, behaviours.ToArray());
            _i++;
            return bot;
        }


        public bool CanCreate => _i < _options.BotsPerRoomCount * _options.RoomsCount;
    }
}