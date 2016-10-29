using System;
using System.Collections;
using System.Collections.Generic;

namespace Conreign.Core.Utility
{
    public class ResetableEnumerator<T> : IEnumerator<T>
    {
        private readonly Func<IEnumerable<T>> _reset;
        private IEnumerator<T> _current;

        public ResetableEnumerator(Func<IEnumerable<T>> reset)
        {
            if (reset == null)
            {
                throw new ArgumentNullException(nameof(reset));
            }
            _reset = reset;
            _current = reset().GetEnumerator();
        }

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
}