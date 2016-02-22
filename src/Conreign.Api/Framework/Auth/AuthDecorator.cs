using System;
using System.Net;
using System.Threading.Tasks;
using Conreign.Api.Framework.ErrorHandling;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Actions;
using Conreign.Core.Contracts.Auth.Data;
using MediatR;
using Newtonsoft.Json.Linq;
using Orleans;

namespace Conreign.Api.Framework.Auth
{
    public class AuthDecorator : IAsyncRequestHandler<GenericAction, GenericActionResult>
    {
        private readonly IAsyncRequestHandler<GenericAction, GenericActionResult> _next;

        public AuthDecorator(IAsyncRequestHandler<GenericAction, GenericActionResult> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task<GenericActionResult> Handle(GenericAction message)
        {
            if (message.Meta == null)
            {
                return await _next.Handle(message);
            }
            var jAccessToken = message.Meta.SelectToken("auth.accessToken");
            if (jAccessToken == null || jAccessToken.Type != JTokenType.String)
            {
                return await _next.Handle(message);
            }
            var accessToken = (string) jAccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                return await _next.Handle(message);
            }
            var meta = new Meta {Auth = new AuthPayload {AccessToken = accessToken}};
            var action = new AuthenticateAction {Meta = meta};
            var auth = GrainClient.GrainFactory.GetGrain<IAuthGrain>(0);
            var result = await auth.Authenticate(action);
            message.Meta["user"] = JObject.FromObject(result);
            return await _next.Handle(message);
        }
    }
}
