using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Conreign.Api.Framework.ErrorHandling;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Conreign.Api.Framework.Owin
{
    internal static class OwinResponseExtensions
    {
        public static Task SendErrorAsync(this IOwinResponse response, IError error, JsonSerializer serializer)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            return SendJsonAsync(response, error, serializer, error.StatusCode);
        }

        public static async Task SendJsonAsync(this IOwinResponse response, object payload, JsonSerializer serializer,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }
            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;
            using (var writer = new StreamWriter(response.Body))
            {
                serializer.Serialize(writer, payload);
                await writer.FlushAsync();
            }
        }
    }
}
