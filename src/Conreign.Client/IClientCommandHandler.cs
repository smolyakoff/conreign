using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using MediatR;

namespace Conreign.Client
{
    public interface IClientCommandHandler : IDisposable
    {
        IObservable<IClientEvent> Events { get; }
        Task<T> Handle<T>(IAsyncRequest<T> command, Metadata metadata);
    }
}