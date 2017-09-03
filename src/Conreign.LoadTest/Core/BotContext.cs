using System;
using Conreign.Client.Contracts;
using Conreign.Contracts.Gameplay;
using Conreign.LoadTest.Core.Events;
using Serilog;

namespace Conreign.LoadTest.Core
{
    public class BotContext
    {
        private readonly Action<Exception> _complete;
        private readonly Action<IBotEvent> _notify;

        internal BotContext(string readableId, IClientConnection connection, Action<Exception> complete,
            Action<IBotEvent> notify)
        {
            if (string.IsNullOrEmpty(readableId))
            {
                throw new ArgumentException("Readable id cannot be null or empty.", nameof(readableId));
            }
            _complete = complete ?? throw new ArgumentNullException(nameof(complete));
            _notify = notify;
            BotId = readableId;
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Logger = Log.Logger
                .ForContext(GetType())
                .ForContext("BotId", readableId)
                .ForContext("ConnectionId", Connection.Id);
        }

        public string BotId { get; }
        public IClientConnection Connection { get; }
        public Guid? UserId { get; set; }
        public IUser User { get; set; }
        public IPlayer Player { get; set; }
        public ILogger Logger { get; set; }

        public void Notify(IBotEvent @event)
        {
            _notify(@event);
        }

        public void Complete(Exception exception = null)
        {
            _complete(exception);
        }
    }
}