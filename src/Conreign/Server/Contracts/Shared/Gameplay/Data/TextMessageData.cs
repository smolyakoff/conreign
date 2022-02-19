using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

[Immutable]
[Serializable]
public class TextMessageData
{
    public string Text { get; set; }
}