using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Conreign.Framework.Http.Core.Data;
using MediatR;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Conreign.Framework.Http.Owin
{
    public class FrameworkDispatcherMiddleware : OwinMiddleware
    {
        private readonly OwinMiddleware _next;

        public FrameworkDispatcherMiddleware(OwinMiddleware next) : base(next)
        {
            _next = next;
        }

        private static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
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
            var scope = context.SurelyGetAutofacLifetimeScope();
            var mediator = scope.Resolve<IMediator>();
            var serializerSettings = scope.Resolve<JsonSerializerSettings>();
            var serializer = JsonSerializer.Create(serializerSettings);
            HttpAction action;
            // TODO: parse encoding header
            using (var reader = new StreamReader(context.Request.Body))
            using (var jsonReader = new JsonTextReader(reader))
            {
                action = serializer.Deserialize<HttpAction>(jsonReader);
            }
            if (string.IsNullOrEmpty(action?.Type))
            {
                await NextOrNotFound(context);
                return;
            }
            var tasks = new[]
            {
                Timeout(),
                mediator.SendAsync(action)
            };
            var response = (await Task.WhenAny(tasks)).Result;
            if (response == null)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NoContent;
                return;
            }
            await context.Response.SendJsonAsync(response.Payload, response.StatusCode);
        }

        private static async Task<HttpActionResult> Timeout()
        {
            var delay = IsDebug ? TimeSpan.FromMinutes(2) : TimeSpan.FromSeconds(10);
            await Task.Delay(delay);
            return new HttpActionResult(HttpStatusCode.GatewayTimeout, null);
        }

        private Task NextOrNotFound(IOwinContext context)
        {
            return _next == null
                ? context.Response.SendNotFoundErrorAsync()
                : _next.Invoke(context);
        }

        private static class Constants
        {
            public const string HttpPost = "POST";

            public const string JsonContentType = "application/json";
        }
    }
}