using System;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Client.Handler.Handlers.Common
{
    internal class HandlerContext : IHandlerContext
    {
        public HandlerContext(IClientConnection connection, Metadata metadata)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            Connection = connection;
            Metadata = metadata;
        }

        public Metadata Metadata { get; }
        public Guid? UserId { get; set; }
        public IUser User { get; set; }
        public IClientConnection Connection { get; }
    }
}