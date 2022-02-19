using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Server.Communication.Events;
using Conreign.Server.Contracts.Shared.Gameplay;
using Conreign.Server.Contracts.Shared.Gameplay.Events;
using Orleans;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IPlayerGrain :
    IGrainWithGuidCompoundKey,
    IPlayer,
    IEventHandler<GameStartedServer>,
    IEventHandler<GameEnded>,
    IEventHandler<Connected>,
    IEventHandler<Disconnected>
{
    Task Ping();
}