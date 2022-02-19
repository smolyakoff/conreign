using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Conreign.Server.Core.Utility;

public sealed class ResetableEnumerator<T> : IEnumerator<T>
{
    private readonly Func<IEnumerable<T>> _reset;
    private IEnumerator<T> _current;

    public ResetableEnumerator(Func<IEnumerable<T>> reset)
    {
        _reset = reset ?? throw new ArgumentNullException(nameof(reset));
        _current = reset().GetEnumerator();
    }

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public void Dispose()
    {
        _current.Dispose();
    }

    public bool MoveNext()
    {
        return _current.MoveNext();
    }

    public void Reset()
    {
        _current = _reset().GetEnumerator();
    }

    public T Current => _current.Current;

    object IEnumerator.Current => _current.Current;
}