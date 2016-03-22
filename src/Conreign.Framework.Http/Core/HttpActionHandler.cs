using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using Conreign.Framework.Http.Core.Data;
using Conreign.Framework.Http.ErrorHandling;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace Conreign.Framework.Http.Core
{
    public class HttpActionHandler : IAsyncRequestHandler<HttpAction, HttpActionResult>
    {
        private readonly ErrorFactory _errorFactory;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly JsonSerializer _serializer;

        public HttpActionHandler(IMediator mediator, ErrorFactory errorFactory, JsonSerializerSettings settings = null)
        {
            if (errorFactory == null)
            {
                throw new ArgumentNullException(nameof(errorFactory));
            }
            _logger = Log.ForContext(GetType());
            _mediator = mediator;
            _errorFactory = errorFactory;
            _serializer = JsonSerializer.Create(settings ?? JsonConvert.DefaultSettings());
        }

        public async Task<HttpActionResult> Handle(HttpAction message)
        {
            try
            {
                var req = new Request {Type = message.Type};
                if (message.Meta != null)
                {
                    req.Meta = message.Meta.ToObject<Dictionary<string, object>>(_serializer);
                }
                if (message.Payload != null)
                {
                    req.Payload = message.Payload.ToObject<Dictionary<string, object>>(_serializer);
                }
                var response = await _mediator.SendAsync(req);
                if (response.IsSuccess)
                {
                    return new HttpActionResult(HttpStatusCode.OK, response.Result);
                }
                var error = _errorFactory.Create(response.Error);
                return new HttpActionResult(error.StatusCode, error);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "HTTP handler failure: {Message}", ex.Message);
                var error = _errorFactory.Create(ex);
                return new HttpActionResult(error.StatusCode, error);
            }
        }
    }
}