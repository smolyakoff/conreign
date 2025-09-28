using Conreign.Server.Contracts.Client;
using Conreign.Server.Contracts.Shared.Gameplay;

namespace Conreign.Server.Api.Handler;

public class HandlerContext
{
    public Metadata Metadata { get; set; }
    public Guid? UserId { get; set; }
    public IUser User { get; set; }
    public IClientConnection Connection { get; set; }
}