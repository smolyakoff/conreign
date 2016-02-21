using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Conreign.Api.Framework.ErrorHandling;
using MediatR;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Conreign.Api.Framework.Owin
{
    public class FrameworkDispatcherMiddleware : OwinMiddleware
    {
        private readonly OwinMiddleware _next;
        private readonly JsonSerializer _serializer;
        private readonly SingleInstanceFactory _factory;

        private static class Constants
        {
            public const string HttpPost = "POST";

            public const string JsonContentType = "application/json";
        }

        public FrameworkDispatcherMiddleware(OwinMiddleware next, FrameworkOptions options) : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _next = next;
            _serializer = JsonSerializer.Create(options.SerializerSettings ?? new JsonSerializerSettings());
            _factory = options.ObjectFactory;
            if (_factory == null)
            {
                throw new ArgumentException("Object factory should be set in configuration.");
            }
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Method != Constants.HttpPost)
            {
                await NextOrNotFound(context);
                return;
            }
            if (context.Request.ContentType != Constants.JsonContentType)
            {
                await NextOrNotFound(context);
                return;
            }
            IMediator mediator;
            GenericAction action;
            // TODO: parse encoding header
            using (var reader = new StreamReader(context.Request.Body))
            using (var jsonReader = new JsonTextReader(reader))
            {
                action = _serializer.Deserialize<GenericAction>(jsonReader);
                mediator = _factory(typeof (IMediator)) as IMediator;
            }
            if (string.IsNullOrEmpty(action?.Type))
            {
                await NextOrNotFound(context);
                return;
            }
            if (mediator == null)
            {
                throw new InvalidOperationException("Could not resolve IMediator.");
            }
            var response = await mediator.SendAsync(action);
            if (response == null)
            {
                throw new InvalidOperationException("Handler returned null as action result.");
            }
            await context.Response.SendJsonAsync(response.Payload, _serializer, response.StatusCode);
        }

        private Task NextOrNotFound(IOwinContext context)
        {
            return _next != null 
                ? _next.Invoke(context) 
                : context.Response.SendErrorAsync(UserError.NotFound(), _serializer);
        }
    }
}
