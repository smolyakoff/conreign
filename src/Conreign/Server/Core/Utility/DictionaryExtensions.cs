namespace Conreign.Server.Core.Utility;

public static class DictionaryExtensions
{
    public static TValue GetOrCreateDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> defaultValueAccessor)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        dictionary[key] = defaultValueAccessor();
        return dictionary[key];
    }

    public static TValue GetOrCreateDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        dictionary[key] = new TValue();
        return dictionary[key];
    }
}