using System;
using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using MediatR;
using SimpleInjector;

namespace Conreign.Client.Handler
{
    public class ClientHandler : IClientHandler
    {
        private readonly IClientConnection _connection;
        private readonly IMediator _mediator;
        private bool _isDisposed;

        public ClientHandler(IClientConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            var container = new Container();
            container.RegisterClientMediator();
            _connection = connection;
            _mediator = container.GetInstance<IMediator>();
        }

        public ClientHandler(IClientConnection connection, IMediator mediator)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public IObservable<IClientEvent> Events => _connection.Events;

        public async Task<T> Handle<T>(IRequest<T> command, Metadata metadata)
        {
            EnsureIsNotDisposed();
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
            var result = await _mediator.Send((dynamic)envelope);
            return (T) result;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _connection.Dispose();
            _isDisposed = true;
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("GameHandler");
            }
        }
    }
}