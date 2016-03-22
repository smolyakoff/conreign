using System;
using System.Threading.Tasks;

namespace Conreign.Core.Game
{
    public class GameRoomKeyGenerator
    {
        private readonly Random _random = new Random();

        public Task<string> Generate()
        {
            var a = _random.Next();
            return Task.FromResult($"{a:X}".ToUpperInvariant());
        }
    }
}