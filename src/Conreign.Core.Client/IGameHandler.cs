using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using MediatR;

namespace Conreign.Core.Client
{
    public interface IGameHandler : IDisposable
    {
        IObservable<IClientEvent> Events { get; }
        Task<T> Handle<T>(IAsyncRequest<T> command, Metadata metadata);
    }
}