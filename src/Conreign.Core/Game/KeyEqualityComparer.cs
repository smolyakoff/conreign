using System;
using System.Collections.Generic;

namespace Conreign.Core.Game
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T> where TKey : IEquatable<TKey>
    {
        private readonly Func<T, TKey> _keySelector;

        public KeyEqualityComparer(Func<T, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return _keySelector(x).Equals(_keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return _keySelector(obj).GetHashCode();
        }
    }
}