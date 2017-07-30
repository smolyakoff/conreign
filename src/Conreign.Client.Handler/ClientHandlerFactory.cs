using System;
using Conreign.Core.Contracts.Client;
using MediatR;
using SimpleInjector;

namespace Conreign.Client.Handler
{
    public class ClientHandlerFactory
    {
        private readonly IMediator _mediator;

        public ClientHandlerFactory(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public IClientHandler Create(IClientConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            return new ClientHandler(connection, _mediator);
        }
    }
}