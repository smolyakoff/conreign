using System;
using System.Net;
using System.Threading.Tasks;
using Conreign.Api.Framework.ErrorHandling;
using Conreign.Core.Contracts;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Actions;
using Conreign.Core.Contracts.Auth.Data;
using Conreign.Core.Contracts.Game;
using MediatR;
using Newtonsoft.Json.Linq;
using Orleans;

namespace Conreign.Api.Framework.Auth
{
    public class AuthDecorator : IAsyncRequestHandler<HttpAction, HttpActionResult>
    {
        private readonly IAsyncRequestHandler<HttpAction, HttpActionResult> _next;

        public AuthDecorator(IAsyncRequestHandler<HttpAction, HttpActionResult> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task<HttpActionResult> Handle(HttpAction message)
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
            var payload = new AccessTokenPayload {AccessToken = accessToken};
            var action = new AuthenticateAction(payload);
            var auth = GrainClient.GrainFactory.GetGrain<IAuthGrain>(0);
            var result = await auth.Authenticate(action);
            message.Meta["auth"] = JObject.FromObject(new AuthMeta {AccessToken = accessToken, ErrorMessage = result.ErrorMessage});
            if (result.UserKey != null)
            {
                message.Meta["user"] = JObject.FromObject(new UserMeta {UserKey = result.UserKey.Value});
            }
            return await _next.Handle(message);
        }
    }
}
