using System;
using System.Threading.Tasks;
using Conreign.Core.Auth.Actions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Game;
using Conreign.Framework.Contracts.Auth;
using JWT;
using MediatR;
using Serilog;

namespace Conreign.Core.Auth
{
    public class AuthService : 
        IAsyncRequestHandler<LoginAnonymousAction, AccessTokenPayload>, 
        IAsyncRequestHandler<AuthenticateAction, AuthenticationResultPayload>
    {
        private readonly AuthOptions _options;
        private readonly ILogger _logger;

        static AuthService()
        {
            JsonWebToken.JsonSerializer = new JsonNetSerializer();
        }

        public AuthService(AuthOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _logger = Log.ForContext(GetType());
            _options = options;
        }

        public Task<AccessTokenPayload> Handle(LoginAnonymousAction action)
        {
            var userId = action.Payload.ToString();
            var lifetime = TimeSpan.FromSeconds(_options.TokenLifetimeInSeconds);
            var payload = JwtTokenPayload.Create(userId, lifetime);
            var token = JsonWebToken.Encode(payload, _options.JwtSecret, JwtHashAlgorithm.HS512);
            var result = new AccessTokenPayload { AccessToken = token };
            return Task.FromResult(result);
        }

        public Task<AuthenticationResultPayload> Handle(AuthenticateAction action)
        {
            var accessToken = action?.Payload?.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token is required to authenticate.");
            }
            JwtTokenPayload payload;
            try
            {
                payload = JsonWebToken.DecodeToObject<JwtTokenPayload>(accessToken, _options.JwtSecret, false);
            }
            catch (Exception ex)
            {
                _logger.Verbose("Error parsing access token: {Message}.", ex.Message);
                return Task.FromResult(new AuthenticationResultPayload
                {
                    Auth = new AuthMeta { Error = AuthenticationError.BadToken, AccessToken = accessToken}
                });
            }
            if (payload.ExpiresAt < DateTime.UtcNow)
            {
                return Task.FromResult(new AuthenticationResultPayload
                {
                    Auth = new AuthMeta { Error = AuthenticationError.TokenExpired, AccessToken = accessToken}
                });
            }
            Guid userKey;
            var parsed = Guid.TryParse(payload.Subject, out userKey);
            if (!parsed)
            {
                return Task.FromResult(new AuthenticationResultPayload
                {
                    Auth = new AuthMeta { Error = AuthenticationError.BadToken, AccessToken = accessToken }
                });
            }
            var result = new AuthenticationResultPayload
            {
                User = new UserMeta {UserKey = userKey},
                Auth = new AuthMeta { AccessToken = accessToken}
            };
            return Task.FromResult(result);
        }
    }
}