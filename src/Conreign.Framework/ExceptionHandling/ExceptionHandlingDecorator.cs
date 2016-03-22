using System;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Core;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using Conreign.Framework.Core;
using MediatR;

namespace Conreign.Framework.ExceptionHandling
{
    public class ExceptionHandlingDecorator : IAsyncRequestHandler<Request, Response>
    {
        private readonly IAsyncRequestHandler<Request, Response> _next;

        public ExceptionHandlingDecorator(IAsyncRequestHandler<Request, Response> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task<Response> Handle(Request message)
        {
            try
            {
                return await _next.Handle(message);
            }
            catch (UserException ex)
            {
                return Response.Problem(UserError.Create(ex));
            }
            catch (AggregateException ex)
            {
                return Response.Problem(ErrorFactory.ForAggregateException(ex));
            }
            catch (Exception ex)
            {
                return Response.Problem(ErrorFactory.ForException(ex));
            }
        }
    }
}