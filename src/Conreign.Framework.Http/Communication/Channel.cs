using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNet.SignalR;
using IRequest = Microsoft.AspNet.SignalR.IRequest;

namespace Conreign.Framework.Http.Communication
{
    public class Channel : PersistentConnection
    {
        private readonly IMediator _mediator;

        public Channel(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
