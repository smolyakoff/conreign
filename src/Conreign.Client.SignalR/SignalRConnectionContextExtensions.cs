using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using MediatR;

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
            if (typeof(T) != typeof(Unit))
            {
                var response = await context.Hub.Invoke<MessageEnvelope>("Send", envelope).ConfigureAwait(false);
                return (T) response.Payload;
            }
            await context.Hub.Invoke("Post", envelope);
            return default(T);
        }
    }
}