using System;

namespace Conreign.Server.Utility
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
