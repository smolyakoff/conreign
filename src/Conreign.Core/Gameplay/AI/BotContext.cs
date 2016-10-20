using System;
using Conreign.Core.Contracts.Gameplay;
using Serilog;

namespace Conreign.Core.Gameplay.AI
{
    public class BotContext
    {
        private readonly Action _complete;

        internal BotContext(string readableId, Guid userId, IUser user, Action complete)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (complete == null)
            {
                throw new ArgumentNullException(nameof(complete));
            }
            if (string.IsNullOrEmpty(readableId))
            {
                throw new ArgumentException("Readable id cannot be null or empty.", nameof(readableId));
            }
            _complete = complete;
            ReadableId = readableId;
            UserId = userId;
            User = user;
            Logger = Log.Logger
                .ForContext("ReadableId", readableId)
                .ForContext("UserId", userId);
        }

        public string ReadableId { get; }
        public Guid UserId { get; }
        public IPlayer Player { get; set; }
        public IUser User { get; }
        public ILogger Logger { get; }

        public void Complete()
        {
            _complete();
        }
    }
}