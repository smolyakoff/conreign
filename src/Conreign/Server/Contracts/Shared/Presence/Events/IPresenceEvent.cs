namespace Conreign.Server.Contracts.Shared.Presence.Events;

public interface IPresenceEvent
{
    string HubId { get; }
}