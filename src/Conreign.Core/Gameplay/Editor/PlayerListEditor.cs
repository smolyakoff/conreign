using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Gameplay.Validators;
using Conreign.Core.Utility;

namespace Conreign.Core.Gameplay.Editor
{
    public class PlayerListEditor
    {
        private readonly PlayerOptionsValidator _optionsValidator;
        private readonly List<PlayerData> _state;

        public PlayerListEditor(List<PlayerData> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _state = state;
            _optionsValidator = new PlayerOptionsValidator();
        }

        public int Count => _state.Count;

        public PlayerData this[Guid userId]
        {
            get
            {
                EnsurePlayerExists(userId);
                return _state.Find(x => x.UserId == userId);
            }
        }

        public bool Contains(Guid userId)
        {
            return _state.Exists(x => x.UserId == userId);
        }

        public PlayerData Add(Guid userId)
        {
            if (Contains(userId))
            {
                throw new InvalidOperationException($"User with id ${userId} has been already added");
            }
            var color = Sequences
                .RandomWithPopularFirstColors
                .FirstOrDefault(c => _state.All(p => p.Color != c));
            var player = new PlayerData
            {
                UserId = userId,
                Color = color,
                Nickname = null
            };
            _state.Add(player);
            return player;
        }

        public bool Update(Guid userId, PlayerOptionsData options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            EnsurePlayerExists(userId);
            options.EnsureIsValid(_optionsValidator);
            var player = this[userId];
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

        private void EnsurePlayerExists(Guid userId)
        {
            if (!Contains(userId))
            {
                throw new InvalidOperationException($"User with id {userId} is not a member");
            }
        }
    }
}