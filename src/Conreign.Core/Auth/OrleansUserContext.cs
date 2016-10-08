using System;
using Conreign.Core.Contracts.Auth;
using Orleans.Runtime;

namespace Conreign.Core.Auth
{
    public class OrleansUserContext : IUserContext
    {
        public Guid ConnectionId => (Guid?) RequestContext.Get("ConnectionId") ?? Guid.Empty;
        public Guid UserId => (Guid?) RequestContext.Get("UserId") ?? Guid.Empty;
    }
}
