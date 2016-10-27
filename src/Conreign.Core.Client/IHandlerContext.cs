using System;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Client
{
    public interface IHandlerContext
    {
        string AccessToken { get; }
        IClientConnection Connection { get; }
        string TraceId { get; }
        Guid? UserId { get; set; }
        IUser User { get; set; }
    }
}