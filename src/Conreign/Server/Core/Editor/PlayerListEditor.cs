﻿using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Conreign.Server.Core.Editor.Validators;
using Conreign.Server.Core.Utility;

namespace Conreign.Server.Core.Editor;

public class PlayerListEditor
{
    private readonly List<PlayerData> _state;

    public PlayerListEditor(List<PlayerData> state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
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
        var existingNicknames = _state.Select(x => x.Nickname).ToHashSet();
        var nickname = Sequences.Nicknames.FirstOrDefault(x => !existingNicknames.Contains(x));
        var player = new PlayerData
        {
            UserId = userId,
            Color = color,
            Nickname = nickname
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
        var usedNicknames = _state
            .Where(x => x.UserId != userId)
            .Select(x => x.Nickname)
            .ToHashSet();
        var validator = new PlayerOptionsValidator(usedNicknames);
        options.EnsureIsValid(validator);
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