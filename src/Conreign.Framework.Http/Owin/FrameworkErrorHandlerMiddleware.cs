using System;
using System.Threading.Tasks;
using Autofac;
using Conreign.Framework.Http.ErrorHandling;
using Microsoft.Owin;
using Serilog;

namespace Conreign.Framework.Http.Owin
{
    public class FrameworkErrorHandlerMiddleware : OwinMiddleware
    {
        private readonly ILogger _logger;
        private readonly OwinMiddleware _next;

        public FrameworkErrorHandlerMiddleware(OwinMiddleware next) : base(next)
        {
            _next = next;
            _logger = Log.ForContext(GetType());
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (_next == null)
            {
                await context.Response.SendNotFoundErrorAsync();
                return;
            }
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "OWIN FAILURE: {Message}", ex.Message);
                var errorFactory = context.SurelyGetAutofacLifetimeScope().Resolve<ErrorFactory>();
                var error = errorFactory.Create(ex);
                await context.Response.SendErrorAsync(error);
            }
        }
    }
}