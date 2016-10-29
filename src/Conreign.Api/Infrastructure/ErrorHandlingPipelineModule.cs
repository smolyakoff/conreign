using System;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace Conreign.Api.Infrastructure
{
    public class ErrorHandlingPipelineModule : HubPipelineModule
    {
        private readonly ILogger _logger;

        public ErrorHandlingPipelineModule(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            _logger = logger.ForContext(GetType());
        }

        protected override void OnIncomingError(ExceptionContext exceptionContext,
            IHubIncomingInvokerContext invokerContext)
        {
            _logger.Error(exceptionContext.Error, $"Exception occurred: {exceptionContext.Error.Message}");
            base.OnIncomingError(exceptionContext, invokerContext);
        }
    }
}