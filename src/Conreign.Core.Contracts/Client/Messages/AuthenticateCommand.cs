using System.Security.Claims;
using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class AuthenticateCommand : IAsyncRequest<ClaimsIdentity>
    {
    }
}
