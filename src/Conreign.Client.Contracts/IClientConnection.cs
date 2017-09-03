using System;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;

namespace Conreign.Client.Contracts
{
    public interface IClientConnection : IDisposable
    {
        Guid Id { get; }
        IObservable<IClientEvent> Events { get; }
        Task<LoginResult> Login(string accessToken = null);
        Task<LoginResult> Authenticate(string accessToken);
    }
}