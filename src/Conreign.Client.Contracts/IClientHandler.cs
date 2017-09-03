using System;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using MediatR;

namespace Conreign.Client.Contracts
{
    public interface IClientHandler
    {
        IObservable<IClientEvent> Events { get; }
        Task<T> Handle<T>(IRequest<T> command, Metadata metadata);
    }
}