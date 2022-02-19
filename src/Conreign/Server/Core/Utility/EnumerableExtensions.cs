﻿namespace Conreign.Server.Core.Utility;

public static class EnumerableExtensions
{
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }

        return new HashSet<T>(source, comparer);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.Shuffle(new Random());
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (random == null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            var j = random.Next(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }
}