using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conreign.LoadTest.Supervisor.Utility
{
    public static class EnumerableExtensions
    {
        public static async Task<IEnumerable<TOutput>> SelectAsync<TInput, TOutput>(
            this IEnumerable<TInput> enumerable,
            Func<TInput, Task<TOutput>> selector,
            int? maxDegreeOfParallelism = null)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            if (maxDegreeOfParallelism == null)
            {
                var results = await Task.WhenAll(enumerable.Select(selector));
                return results;
            }
            var inputPairs = enumerable.Select((x, i) => new KeyValuePair<int, TInput>(i, x)).ToList();
            var queue = new ConcurrentQueue<KeyValuePair<int, TInput>>(inputPairs);
            var tasks = Enumerable.Range(0, maxDegreeOfParallelism.Value)
                .Select(async _ =>
                {
                    KeyValuePair<int, TInput> item;
                    var results = new List<KeyValuePair<int, TOutput>>();
                    while (queue.TryDequeue(out item))
                    {
                        var value = await selector(item.Value);
                        results.Add(new KeyValuePair<int, TOutput>(item.Key, value));
                    }
                    return results;
                })
                .ToList();
            var outputPairLists = await Task.WhenAll(tasks);
            return outputPairLists.SelectMany(x => x).OrderBy(x => x.Key).Select(x => x.Value);
        }
    }
}