using System;
using System.Threading.Tasks;
using Conreign.Client.Contracts;
using Conreign.Contracts.Communication;
using MediatR;

namespace Conreign.Client.Handler
{
    public class ClientHandler : IClientHandler
    {
        private readonly IClientConnection _connection;
        private readonly IMediator _mediator;

        public ClientHandler(IClientConnection connection, IMediator mediator)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public IObservable<IClientEvent> Events => _connection.Events;

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
            var context = new HandlerContext(_connection, metadata);
            var envelopeType = typeof(CommandEnvelope<,>).MakeGenericType(command.GetType(), typeof(T));
            var envelope = Activator.CreateInstance(envelopeType, command, context);
            var result = await _mediator.Send((dynamic) envelope);
            return (T) result;
        }
    }
}