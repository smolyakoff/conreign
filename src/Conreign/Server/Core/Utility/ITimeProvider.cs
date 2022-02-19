namespace Conreign.Server.Core.Utility;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}