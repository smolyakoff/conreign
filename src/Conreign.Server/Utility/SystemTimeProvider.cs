using System;

namespace Conreign.Server.Utility
{
    internal class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
