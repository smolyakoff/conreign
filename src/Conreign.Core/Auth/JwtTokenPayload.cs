using System;
using Conreign.Core.Utility;
using Newtonsoft.Json;

namespace Conreign.Core.Auth
{
    public class JwtTokenPayload
    {
        public static JwtTokenPayload Create(string subject, TimeSpan lifetime, string audience = "conreign-client")
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("Subject should not be empty.", nameof(subject));
            }
            if (lifetime.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lifetime));
            }
            var now = DateTime.UtcNow;
            var expires = now.Add(lifetime);
            return new JwtTokenPayload(subject, now, expires, audience);
        }

        private JwtTokenPayload(string subject, DateTime issuedAt, DateTime expiresAt, string audience)
        {
            Id = Guid.NewGuid().ToString("N");
            Issuer = "conreign";
            Subject = subject;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            Audience = audience;
        }

        [JsonConstructor]
        private JwtTokenPayload()
        {
        }

        [JsonProperty(PropertyName = "jti")]
        public string Id { get; private set; }

        [JsonProperty(PropertyName = "aud")]
        public string Audience { get; private set; }

        [JsonProperty(PropertyName = "iss")]
        public string Issuer { get; private set; }

        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; private set; }

        [JsonProperty(PropertyName = "exp")]
        [JsonConverter(typeof(SecondsSinceEpochConverter))]
        public DateTime ExpiresAt { get; private set; }

        [JsonProperty(PropertyName = "iat")]
        [JsonConverter(typeof(SecondsSinceEpochConverter))]
        public DateTime IssuedAt { get; private set; }
    }
}
