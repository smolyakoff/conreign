using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Contracts.Auth;
using Conreign.Contracts.Errors;
using Conreign.Server.Auth.Serialization;
using Conreign.Server.Contracts.Auth;
using JWT;
using Serilog;

namespace Conreign.Server.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly AuthOptions _options;

        public AuthService(AuthOptions options)
        {
            JsonWebToken.JsonSerializer = new JsonNetSerializer();
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = Log.Logger.ForContext(GetType());
        }

        public Task<ClaimsIdentity> Authenticate(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }
            ClaimsIdentity identity;
            var exception = TryAuthenticate(accessToken, out identity);
            if (exception != null)
            {
                throw exception;
            }
            return Task.FromResult(identity);
        }

        public Task<string> Login(string accessToken = null)
        {
            ClaimsIdentity identity;
            Guid userId;
            if (string.IsNullOrEmpty(accessToken) || TryAuthenticate(accessToken, out identity) != null)
            {
                userId = Guid.NewGuid();
            }
            else
            {
                var userIdString = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var parsed = Guid.TryParse(userIdString, out userId);
                userId = parsed ? userId : Guid.NewGuid();
            }
            var lifetime = TimeSpan.FromSeconds(_options.TokenLifetimeInSeconds);
            var payload = JwtTokenPayload.Create(userId.ToString(), lifetime);
            var token = JsonWebToken.Encode(payload, _options.JwtSecret, JwtHashAlgorithm.HS512);
            return Task.FromResult(token);
        }

        private UserException TryAuthenticate(string accessToken, out ClaimsIdentity identity)
        {
            JwtTokenPayload payload;
            identity = null;
            try
            {
                payload = JsonWebToken.DecodeToObject<JwtTokenPayload>(accessToken, _options.JwtSecret, false);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, $"Failed to parse authentication token: {ex.Message}");
                return UserException.Create(AuthenticationError.BadToken, "Invalid authentication token.");
            }
            if (payload.ExpiresAt < DateTime.UtcNow)
            {
                return UserException.Create(AuthenticationError.ExpiredToken, "Authentication token is expired.");
            }
            Guid userId;
            var parsed = Guid.TryParse(payload.Subject, out userId);
            if (!parsed)
            {
                return UserException.Create(AuthenticationError.BadToken, "Invalid subject.");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            identity = new ClaimsIdentity(claims);
            return null;
        }
    }
}