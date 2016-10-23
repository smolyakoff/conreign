using System;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Client
{
    public interface IHandlerContext
    {
        string AccessToken { get; }
        IGameConnection Connection { get; }
        string TraceId { get; }
        Guid? UserId { get; set; }
        IUser User { get; set; }
    }
}