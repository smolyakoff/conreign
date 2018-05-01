using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Utility;

namespace Conreign.Core.Editor
{
    public class PlayerListEditor
    {
        private readonly List<PlayerData> _state;

        public PlayerListEditor(List<PlayerData> state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public int PlayersCount => _state.Count;
        public int BotsCount => _state.Count(x => x.Type == PlayerType.Bot);
        public int HumansCount => _state.Count(x => x.Type == PlayerType.Human);
        public IEnumerable<Guid> PlayerIds => _state.Select(x => x.UserId);
        public IEnumerable<Guid> BotIds => _state.Where(x => x.Type == PlayerType.Bot).Select(x => x.UserId);

        public PlayerData this[Guid userId]
        {
            get
            {
                EnsurePlayerExists(userId);
                return _state.Find(x => x.UserId == userId);
            }
        }

        public bool ContainsPlayerWithUserId(Guid userId)
        {
            return _state.Exists(x => x.UserId == userId);
        }

        public bool ContainsBotWithUserId(Guid userId)
        {
            return _state.Exists(x => x.Type == PlayerType.Bot && x.UserId == userId);
        }

        public PlayerData AddHuman(Guid userId)
        {
            if (ContainsPlayerWithUserId(userId))
            {
                throw new InvalidOperationException($"User with id ${userId} has been already added");
            }
            var player = GenerateHuman(userId);
            _state.Add(player);
            return player;
        }

        public (List<PlayerData> BotsAdded, List<PlayerData> BotsRemoved) AdjustBotCount(int desiredBotsCount)
        {
            if (desiredBotsCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(desiredBotsCount), 
                    desiredBotsCount,
                    "Desired bots count should be 0 or greater.");
            }
            if (BotsCount == desiredBotsCount)
            {
                return (new List<PlayerData>(0), new List<PlayerData>(0));
            }
            var difference = Math.Abs(desiredBotsCount - BotsCount);
            if (BotsCount > desiredBotsCount)
            {
                var botsToRemove = _state
                    .Where(x => x.Type == PlayerType.Bot)
                    .Take(difference)
                    .ToList();
                foreach (var bot in botsToRemove)
                {
                    _state.Remove(bot);
                }
                return (new List<PlayerData>(0), botsToRemove);
            }
            var botsToAdd = GenerateBots(difference);
            _state.AddRange(botsToAdd);
            return (botsToAdd, new List<PlayerData>(0));
        }

        public bool UpdateHumanOptions(Guid userId, PlayerOptionsData options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (!ContainsPlayerWithUserId(userId))
            {
                throw new ArgumentException($"Player with id {userId} does not exist.", nameof(userId));
            }
            var player = this[userId];
            if (player.Type != PlayerType.Human)
            {
                throw new ArgumentException("User id should reference human player.", nameof(userId));
            }
            var usedNicknames = _state
                .Where(x => x.UserId != userId)
                .Select(x => x.Nickname)
                .ToHashSet();
            var validator = new PlayerOptionsValidator(usedNicknames);
            validator.EnsureIsValid(options);
            var current = new PlayerOptionsData
            {
                Nickname = player.Nickname,
                Color = player.Color
            };
            if (current == options)
            {
                return false;
            }
            player.Nickname = options.Nickname;
            player.Color = options.Color;
            return true;
        }

        private PlayerData GenerateHuman(Guid userId)
        {
            var player = new PlayerData
            {
                UserId = userId,
                Color = GenerateUniqueColors(1)[0],
                Nickname = GenerateUniqueNicknames(1)[0],
                Type = PlayerType.Human,
            };
            return player;
        }

        private List<PlayerData> GenerateBots(int count)
        {
            var nicknames = GenerateUniqueNicknames(count);
            var colors = GenerateUniqueColors(count);
            var players = new List<PlayerData>();
            for (var i = 0; i < count; i++)
            {
                var player = new PlayerData
                {
                    UserId = Guid.NewGuid(),
                    Nickname = nicknames[i],
                    Color = colors[i],
                    Type = PlayerType.Bot,
                };
                players.Add(player);
            }

            return players;
        }

        private List<string> GenerateUniqueNicknames(int count)
        {
            var existingNicknames = _state.Select(x => x.Nickname).ToHashSet();
            return Sequences.RandomNicknames
                .Where(x => !existingNicknames.Contains(x))
                .Take(count)
                .ToList();
        }

        private List<string> GenerateUniqueColors(int count)
        {
            var existingColors = _state.Select(x => x.Color).ToHashSet();
            return Sequences
                .RandomWithPopularFirstColors
                .Where(color => !existingColors.Contains(color))
                .Take(count)
                .ToList();
        }

        private void EnsurePlayerExists(Guid userId)
        {
            if (!ContainsPlayerWithUserId(userId))
            {
                throw new InvalidOperationException($"User with id {userId} is not a member");
            }
        }
    }
}