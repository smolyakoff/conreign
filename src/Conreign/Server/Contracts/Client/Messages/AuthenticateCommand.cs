using System.Security.Claims;
using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class AuthenticateCommand : IRequest<ClaimsIdentity>
{
}