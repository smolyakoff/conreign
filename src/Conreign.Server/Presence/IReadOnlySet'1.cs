using System.Collections.Generic;

namespace Conreign.Server.Presence
{
    public interface IReadOnlySet<T> : IEnumerable<T>
    {
        bool Contains(T userId);
    }
}