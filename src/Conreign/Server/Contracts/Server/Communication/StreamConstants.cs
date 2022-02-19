namespace Conreign.Server.Contracts.Server.Communication;

public class StreamConstants
{
    public const string ProviderName = "DefaultStream";
    private const string ClientNamespace = "conreign/clients";

    public static string GetClientNamespace(string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
        {
            throw new ArgumentException(connectionId, nameof(connectionId));
        }

        return $"{ClientNamespace}/{connectionId}";
    }
}