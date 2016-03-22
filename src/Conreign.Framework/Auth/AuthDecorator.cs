using System;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Auth;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using Conreign.Framework.Core.Serialization;
using MediatR;

namespace Conreign.Framework.Auth
{
    public class AuthDecorator : IAsyncRequestHandler<Request, Response>
    {
        private readonly IConverter _converter;
        private readonly IMediator _mediator;
        private readonly IAsyncRequestHandler<Request, Response> _next;

        public AuthDecorator(IAsyncRequestHandler<Request, Response> next, IMediator mediator, IConverter converter)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }
            _next = next;
            _mediator = mediator;
            _converter = converter;
        }

        public async Task<Response> Handle(Request message)
        {
            if (message.Meta == null)
            {
                return await _next.Handle(message);
            }
            object auth;
            var hasAuth = message.Meta.TryGetValue("auth", out auth);
            if (!hasAuth || auth == null)
            {
                return await _next.Handle(message);
            }
            var tokenPaylod = _converter.Convert<AccessTokenPayload>(auth);
            if (string.IsNullOrEmpty(tokenPaylod?.AccessToken))
            {
                return await _next.Handle(message);
            }
            var authAction = new AuthenticateAction(tokenPaylod);
            var result = await _mediator.SendAsync(authAction);
            message.Meta["user"] = result.User;
            message.Meta["auth"] = result.Auth;
            return await _next.Handle(message);
        }
    }
}