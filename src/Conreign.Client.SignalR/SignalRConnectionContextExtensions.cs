using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Exceptions;
using Conreign.Core.Contracts.Client.Messages;
using MediatR;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;

namespace Conreign.Client.SignalR
{
    internal static class SignalRConnectionContextExtensions
    {
        public static async Task<T> Send<T>(this SignalRConnectionContext context, IAsyncRequest<T> command)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var envelope = new MessageEnvelope
            {
                Meta = context.Metadata,
                Payload = command
            };
            try
            {
                if (typeof(T) != typeof(Unit))
                {
                    var response = await context.Hub.Invoke<MessageEnvelope>("Send", envelope).ConfigureAwait(false);
                    return (T) response.Payload;
                }
                await context.Hub.Invoke("Post", envelope);
                return default(T);
            }
            catch (HubException ex)
            {
                var jError =  ex.ErrorData as JObject;
                if (jError == null)
                {
                    throw new ServerErrorException(ex);
                }
                var typeToken = jError["$nettype"];
                if (typeToken == null)
                {
                    throw new ServerErrorException(ex);
                }
                var type = Type.GetType(typeToken.ToString());
                var error = jError.ToObject(type) as UserError;
                if (error == null)
                {
                    throw new ServerErrorException(ex);
                }
                throw error.ToUserException();
            }
        }
    }
}