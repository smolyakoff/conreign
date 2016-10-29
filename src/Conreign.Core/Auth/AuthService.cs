using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Core.Auth.Serialization;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Errors;
using Conreign.Core.Contracts.Exceptions;
using JWT;
using Serilog;

namespace Conreign.Core.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly AuthOptions _options;

        public AuthService(AuthOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            JsonWebToken.JsonSerializer = new JsonNetSerializer();
            _options = options;
            _logger = Log.Logger.ForContext(GetType());
        }

        public Task<ClaimsIdentity> Authenticate(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }
            JwtTokenPayload payload;
            try
            {
                payload = JsonWebToken.DecodeToObject<JwtTokenPayload>(accessToken, _options.JwtSecret, false);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, $"Failed to parse authentication token: {ex.Message}");
                throw UserException.Create(AuthenticationError.BadToken, "Invalid authentication token.");
            }
            if (payload.ExpiresAt < DateTime.UtcNow)
            {
                throw UserException.Create(AuthenticationError.ExpiredToken, "Authentication token is expired.");
            }
            Guid userId;
            var parsed = Guid.TryParse(payload.Subject, out userId);
            if (!parsed)
            {
                throw UserException.Create(AuthenticationError.BadToken, "Invalid subject.");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            return Task.FromResult(identity);
        }

        public Task<string> Login()
        {
            var userId = Guid.NewGuid();
            var lifetime = TimeSpan.FromSeconds(_options.TokenLifetimeInSeconds);
            var payload = JwtTokenPayload.Create(userId.ToString(), lifetime);
            var token = JsonWebToken.Encode(payload, _options.JwtSecret, JwtHashAlgorithm.HS512);
            return Task.FromResult(token);
        }
    }
}