using System.Collections.Generic;

namespace Conreign.Framework.Contracts.Routing
{
    public interface IRoutedEventEnvelope<out T>
    {
        HashSet<string> ConnectionIds { get; }

        T Data { get; }
    }
}