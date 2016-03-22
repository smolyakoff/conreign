using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Conreign.Core.Game
{
    public sealed class PlayerNameGenerator : IDisposable
    {
        private const string ApiUrl = "https://randomuser.me/api/";

        private readonly HttpClient _client;

        public PlayerNameGenerator()
        {
            _client = new HttpClient();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<string> Generate()
        {
            var response = await _client.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadAsAsync<JObject>();
            var username = (string) user.SelectToken("results[0].user.username");
            return string.IsNullOrEmpty(username)
                ? $"reigner-{new DateTime().ToString("yy-MM-dd")}-{new Random().Next(1, 100)}"
                : new string(username.Where(char.IsLetter).ToArray());
        }
    }
}