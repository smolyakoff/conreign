using System;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Authentication;
using Conreign.Framework.Contracts.Communication;
using MediatR;

namespace Conreign.Framework.Authentication
{
    public class AuthenticationDecorator<TIn, TMeta, TOut> : IAsyncRequestHandler<IMessage<TIn, TMeta, TOut>, TOut>
    {
        private readonly IMediator _mediator;
        private readonly IAsyncRequestHandler<IMessage<TIn, TMeta, TOut>, TOut> _next;

        public AuthenticationDecorator(IAsyncRequestHandler<IMessage<TIn, TMeta, TOut>, TOut> next, IMediator mediator)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            _next = next;
            _mediator = mediator;
        }

        public async Task<TOut> Handle(IMessage<TIn, TMeta, TOut> message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            var meta = message.Meta as IAuthenticationMetadata;
            if (meta == null)
            {
                return await _next.Handle(message);
            }
            var request = new AuthenticateRequest(meta.AccessToken);
            var response = await _mediator.SendAsync(request).ConfigureAwait(false);
            meta.Principal = response.Principal;
            meta.AuthenticationError = response.Error;
            return await _next.Handle(message);
        }
    }
}