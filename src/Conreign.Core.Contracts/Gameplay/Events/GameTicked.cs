﻿using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Immutable]
    [Serializable]
    public class GameTicked : IClientEvent, IRoomEvent
    {
        public GameTicked(string roomId, int tick)
        {
            Tick = tick;
            RoomId = roomId;
        }

        public int Tick { get; }
        public string RoomId { get; }
    }
}