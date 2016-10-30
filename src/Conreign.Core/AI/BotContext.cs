using System;
using Conreign.Core.AI.Events;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Gameplay;
using Serilog;

namespace Conreign.Core.AI
{
    public class BotContext
    {
        private readonly Action _complete;
        private readonly Action<IBotEvent> _notify;

        internal BotContext(string readableId, IClientConnection connection, Action complete, Action<IBotEvent> notify)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrEmpty(readableId))
            {
                throw new ArgumentException("Readable id cannot be null or empty.", nameof(readableId));
            }
            if (complete == null)
            {
                throw new ArgumentNullException(nameof(complete));
            }
            _complete = complete;
            _notify = notify;
            ReadableId = readableId;
            Connection = connection;
            Logger = Log.Logger
                .ForContext("ReadableId", readableId)
                .ForContext("ConnectionId", Connection.Id);
        }

        public string ReadableId { get; }
        public IClientConnection Connection { get; }
        public Guid? UserId { get; set; }
        public IUser User { get; set; }
        public IPlayer Player { get; set; }
        public ILogger Logger { get; set; }

        public void Notify(IBotEvent @event)
        {
            _notify(@event);
        }

        public void Complete()
        {
            _complete();
        }
    }
}