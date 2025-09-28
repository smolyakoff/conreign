using Conreign.Server.Contracts.Client;
using MediatR;

namespace Conreign.Server.Api.Handler;

public class ClientHandler : IClientHandler
{
    private readonly IClientConnection _connection;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ClientHandler(IClientConnection connection, IServiceScopeFactory serviceScopeFactory)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public async Task<T> Handle<T>(IRequest<T> command, Metadata metadata)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (metadata == null)
        {
            throw new ArgumentNullException(nameof(metadata));
        }

        if (string.IsNullOrEmpty(metadata.TraceId))
        {
            metadata.TraceId = Guid.NewGuid().ToString();
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HandlerContext>();
        context.Connection = _connection;
        context.Metadata = metadata;
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);
        return result;
    }
}