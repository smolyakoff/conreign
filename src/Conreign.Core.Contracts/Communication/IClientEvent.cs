using System;

namespace Conreign.Core.Contracts.Communication
{
    public interface IClientEvent : IEvent
    {
        DateTime Timestamp { get; }
    }
}