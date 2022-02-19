namespace Conreign.Server.Contracts.Client;

public interface IClientConnection : IDisposable
{
    string Id { get; }
    Task<LoginResult> Login(string accessToken = null);
    Task<LoginResult> Authenticate(string accessToken);
}