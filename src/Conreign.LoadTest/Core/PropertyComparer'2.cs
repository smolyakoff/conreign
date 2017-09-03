using System;
using System.Collections.Generic;

namespace Conreign.LoadTest.Core
{
    public class PropertyComparer<TObject, TProperty> : IEqualityComparer<TObject>
        where TProperty : IEquatable<TProperty>
    {
        private readonly Func<TObject, TProperty> _selector;

        public PropertyComparer(Func<TObject, TProperty> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            _selector = selector;
        }

        public bool Equals(TObject x, TObject y)
        {
            return _selector(x).Equals(_selector(y));
        }

        public int GetHashCode(TObject obj)
        {
            return _selector(obj).GetHashCode();
        }
    }
}