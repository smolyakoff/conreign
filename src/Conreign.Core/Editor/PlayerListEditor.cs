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

        public (IEnumerable<PlayerData> BotsAdded, IEnumerable<PlayerData> BotsRemoved) AdjustBotCount(int desiredBotsCount)
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
                return (Enumerable.Empty<PlayerData>(), Enumerable.Empty<PlayerData>());
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
                return (Enumerable.Empty<PlayerData>(), botsToRemove);
            }
            var botsToAdd = Enumerable.Range(0, difference)
                .Select(_ => GenerateBot())
                .ToList();
            _state.AddRange(botsToAdd);
            return (botsToAdd, Enumerable.Empty<PlayerData>());
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
            return GeneratePlayer(userId, PlayerType.Human);
        }

        private PlayerData GenerateBot()
        {
            return GeneratePlayer(Guid.NewGuid(), PlayerType.Bot);
        }

        private PlayerData GeneratePlayer(Guid userId, PlayerType type)
        {
            var player = new PlayerData
            {
                UserId = userId,
                Color = GenerateColor(),
                Nickname = GenerateNickname(),
                Type = type
            };
            return player;
        }

        private string GenerateNickname()
        {
            var existingNicknames = _state.Select(x => x.Nickname).ToHashSet();
            var nickname = Sequences.Nicknames.FirstOrDefault(x => !existingNicknames.Contains(x));
            return nickname;
        }

        private string GenerateColor()
        {
            return Sequences
                .RandomWithPopularFirstColors
                .FirstOrDefault(c => _state.All(p => p.Color != c));
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