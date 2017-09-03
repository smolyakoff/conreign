using System;
using Conreign.Client.Contracts;
using Conreign.Contracts.Gameplay;

namespace Conreign.Client.Handler
{
    public class HandlerContext
    {
        internal HandlerContext(IClientConnection connection, Metadata metadata)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public Metadata Metadata { get; }
        public Guid? UserId { get; set; }
        public IUser User { get; set; }
        public IClientConnection Connection { get; }
    }
}