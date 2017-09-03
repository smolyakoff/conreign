using System.Security.Claims;
using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class AuthenticateCommand : IRequest<ClaimsIdentity>
    {
    }
}