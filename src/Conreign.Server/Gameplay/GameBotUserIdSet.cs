using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Server.Presence;

namespace Conreign.Server.Gameplay
{
    internal class GameBotUserIdSet : IReadOnlySet<Guid>
    {
        private readonly IGameState _state;

        public GameBotUserIdSet(IGameState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        private IEnumerable<Guid> BotUserIds => _state
            .Players
            .Where(x => x.Type == PlayerType.Bot)
            .Select(x => x.UserId);
        
        public bool Contains(Guid userId)
        {
            return _state.Status != GameStatus.Pending &&
                   BotUserIds.Any(x => x == userId);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _state.Status == GameStatus.Pending
                ? Enumerable.Empty<Guid>().GetEnumerator()
                : BotUserIds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
