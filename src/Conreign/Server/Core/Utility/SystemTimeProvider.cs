namespace Conreign.Server.Core.Utility;

internal class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}