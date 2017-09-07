using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Contracts.Auth;
using Conreign.Contracts.Errors;
using Conreign.Server.Contracts.Auth;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Serilog;

namespace Conreign.Server.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly AuthOptions _options;
        private readonly IJwtEncoder _jwtEncoder;
        private readonly IJwtDecoder _jwtDecoder;

        public AuthService(AuthOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = Log.Logger.ForContext(GetType());
            var jwtSerializer = new JsonNetSerializer();
            var jwtValidator = new JwtValidator(jwtSerializer, new UtcDateTimeProvider());
            var jwtUrlEncoder = new JwtBase64UrlEncoder();
            _jwtEncoder = new JwtEncoder(new HMACSHA256Algorithm(), jwtSerializer, jwtUrlEncoder);
            _jwtDecoder = new JwtDecoder(jwtSerializer, jwtValidator, jwtUrlEncoder);
        }

        public Task<ClaimsIdentity> Authenticate(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }
            var exception = TryAuthenticate(accessToken, out ClaimsIdentity identity);
            if (exception != null)
            {
                throw exception;
            }
            return Task.FromResult(identity);
        }

        public Task<string> Login(string accessToken = null)
        {
            Guid userId;
            if (string.IsNullOrEmpty(accessToken) || TryAuthenticate(accessToken, out ClaimsIdentity identity) != null)
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
            var token = _jwtEncoder.Encode(payload, _options.JwtSecret);
            return Task.FromResult(token);
        }

        private UserException TryAuthenticate(string accessToken, out ClaimsIdentity identity)
        {
            JwtTokenPayload payload;
            identity = null;
            try
            {
                payload = _jwtDecoder.DecodeToObject<JwtTokenPayload>(accessToken, _options.JwtSecret, false);
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
            var parsed = Guid.TryParse(payload.Subject, out Guid userId);
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