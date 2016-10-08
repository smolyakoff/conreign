using System;

namespace Conreign.Core.Contracts.Auth
{
    public interface IUserContext
    {
        Guid ConnectionId { get; }
        Guid UserId { get; }
    }
}
