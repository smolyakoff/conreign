using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Actions;
using Conreign.Core.Contracts.Auth.Data;
using JWT;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace Conreign.Core.Auth
{
    [StatelessWorker]
    public class AuthGrain : Grain, IAuthGrain
    {
        private const JwtHashAlgorithm HashAlgorithm = JwtHashAlgorithm.HS512;
        private readonly AuthOptions _options;
        private Logger _logger;

        static AuthGrain()
        {
            JsonWebToken.JsonSerializer = new JsonNetSerializer();
        }

        public AuthGrain(AuthOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            _logger = GetLogger();
        }

        public Task<AccessTokenPayload> LoginAnonymous(LoginAnonymousAction action)
        {
            var userId = action.Payload.ToString();
            var lifetime = TimeSpan.FromSeconds(_options.TokenLifetimeInSeconds);
            var payload = JwtTokenPayload.Create(userId, lifetime);
            var token = JsonWebToken.Encode(payload, _options.JwtSecret, HashAlgorithm);
            var result = new AccessTokenPayload {AccessToken = token};
            return Task.FromResult(result);
        }

        public Task<AuthenticationStatusPayload> Authenticate(AuthenticateAction action)
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
                _logger.Verbose($"Error parsing access token: {ex.Message}.");
                return Task.FromResult(new AuthenticationStatusPayload {ErrorMessage = AuthErrors.BadToken});
            }
            
            if (payload.ExpiresAt < DateTime.UtcNow)
            {
                return Task.FromResult(new AuthenticationStatusPayload {ErrorMessage = AuthErrors.TokenExpired});
            }
            var result = new AuthenticationStatusPayload { UserKey = Guid.Parse(payload.Subject) };
            return Task.FromResult(result);
        }
    }
}
