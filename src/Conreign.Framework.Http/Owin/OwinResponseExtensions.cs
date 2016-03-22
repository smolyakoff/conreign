using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Conreign.Framework.Http.ErrorHandling;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Conreign.Framework.Http.Owin
{
    internal static class OwinResponseExtensions
    {
        public static Task SendNotFoundErrorAsync(this IOwinResponse response)
        {
            var error = new HttpError
            {
                Message = "Requested resource was not found.",
                Type = "NotFound",
                StatusCode = HttpStatusCode.NotFound
            };
            return SendErrorAsync(response, error);
        }

        public static Task SendErrorAsync(this IOwinResponse response, HttpError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            return SendJsonAsync(response, error, error.StatusCode);
        }

        public static async Task SendJsonAsync(this IOwinResponse response, object payload,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var settings = response.Context.SurelyGetAutofacLifetimeScope().Resolve<JsonSerializerSettings>();
            var serializer = JsonSerializer.Create(settings);
            response.ContentType = "application/json";
            response.StatusCode = (int) statusCode;
            using (var writer = new StreamWriter(response.Body))
            {
                serializer.Serialize(writer, payload);
                await writer.FlushAsync();
            }
        }
    }
}