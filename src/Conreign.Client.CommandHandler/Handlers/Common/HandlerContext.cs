using System;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Client.CommandHandler.Handlers.Common
{
    internal class HandlerContext : IHandlerContext
    {
        private readonly Metadata _metadata;

        public HandlerContext(IClientConnection connection, Metadata metadata, string traceId)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            if (string.IsNullOrEmpty(traceId))
            {
                throw new ArgumentException("Trace id cannot be null or empty.", nameof(traceId));
            }
            _metadata = metadata;
            Connection = connection;
            TraceId = traceId;
        }

        public string AccessToken => _metadata.AccessToken;
        public Guid? UserId { get; set; }
        public IUser User { get; set; }
        public string TraceId { get; }
        public IClientConnection Connection { get; }
    }
}