using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Client
{
    public interface IGameConnection : IDisposable
    {
        Guid Id { get; }
        IObservable<IClientEvent> Events { get; }
        Task<LoginResult> Login();
        Task<LoginResult> Authenticate(string accessToken);
    }

}