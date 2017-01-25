using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using MediatR;

namespace Conreign.Core.Contracts.Client
{
    public interface IClientHandler : IDisposable
    {
        IObservable<IClientEvent> Events { get; }
        Task<T> Handle<T>(IRequest<T> command, Metadata metadata);
    }
}