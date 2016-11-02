using System;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Client.Handler.Handlers.Common
{
    internal interface IHandlerContext
    {
        Metadata Metadata { get; }
        IClientConnection Connection { get; }
        Guid? UserId { get; set; }
        IUser User { get; set; }
    }
}