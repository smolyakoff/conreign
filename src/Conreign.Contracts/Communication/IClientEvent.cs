using System;

namespace Conreign.Contracts.Communication
{
    public interface IClientEvent : IEvent
    {
        DateTime Timestamp { get; }
    }
}