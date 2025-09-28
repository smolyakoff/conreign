using MediatR;

namespace Conreign.Server.Contracts.Client;

public interface IClientHandler
{
    Task<T> Handle<T>(IRequest<T> command, Metadata metadata);
}