using System;

namespace Conreign.Framework.Core.Serialization
{
    public interface IConverter
    {
        T Convert<T>(object source);

        object Convert(object source, Type targetType);
    }
}