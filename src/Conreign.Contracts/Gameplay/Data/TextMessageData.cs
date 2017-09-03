using System;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Data
{
    [Immutable]
    [Serializable]
    public class TextMessageData
    {
        public string Text { get; set; }
    }
}