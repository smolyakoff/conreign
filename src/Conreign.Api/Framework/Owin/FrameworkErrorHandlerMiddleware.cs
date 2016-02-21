using System;
using System.Threading.Tasks;
using Conreign.Api.Framework.ErrorHandling;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Conreign.Api.Framework.Owin
{
    public class FrameworkErrorHandlerMiddleware : OwinMiddleware
    {
        private readonly OwinMiddleware _next;
        private readonly JsonSerializer _serializer;
        private readonly ErrorFactory _errorFactory;

        public FrameworkErrorHandlerMiddleware(OwinMiddleware next, FrameworkOptions options) : base(next)
        {
            _next = next;
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _errorFactory = new ErrorFactory(new ErrorFactorySettings
            {
                SerializeStackTrace = options.Debug,
                SerializeSystemErrorMessage = options.Debug
            });
            _serializer = JsonSerializer.Create(options.SerializerSettings ?? new JsonSerializerSettings());
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (_next == null)
            {
                await context.Response.SendErrorAsync(UserError.NotFound(), _serializer);
                return;
            }
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var error = _errorFactory.Create(ex);
                await context.Response.SendErrorAsync(error, _serializer);
            }
        }
    }
}
