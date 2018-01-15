using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Utility;

namespace Conreign.Core.Editor
{
    public static class Sequences
    {
        private static readonly HashSet<int> PopularColors = new HashSet<int>
        {
            0xff0000,
            0x008080,
            0xffe4e1,
            0x0000ff,
            0xd3ffce,
            0x40e0d0,
            0xffd700,
            0xff7373,
            0xb0e0e6,
            0xcccccc,
            0x666666,
            0xf6546a,
            0x333333,
            0x003366,
            0xffa500,
            0x800080,
            0x66cdaa,
            0x800000,
            0x008000,
            0xf08080,
            0xcc0000
        };

        public static IEnumerable<string> RandomWithPopularFirstColors
        {
            get
            {
                foreach (var color in PopularColors.Shuffle())
                    yield return $"#{color:x6}";
                var random = new Random();
                var used = new HashSet<int>(PopularColors);
                while (used.Count < 0xffffff + 1)
                {
                    int next;
                    do
                    {
                        next = random.Next(0, 0xffffff + 1);
                    } while (used.Contains(next));
                    used.Add(next);
                    yield return $"#{next:x6}";
                }
            }
        }

        public static IEnumerable<string> Colors => Enumerable.Range(0, 0xffffff + 1).Select(x => $"#{x:x6}");

        public static IEnumerable<string> PlanetNames
        {
            get
            {
                var buffer = new List<string> {""};
                while (true)
                {
                    var output = new List<string>();
                    foreach (var item in buffer)
                    foreach (var letter in Alphabet)
                    {
                        var name = string.Concat(item, letter);
                        output.Add(name);
                        yield return name;
                    }
                    buffer = output;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }

        private static IEnumerable<string> Alphabet
        {
            get
            {
                var c = 'A';
                while (c < 'Z')
                {
                    yield return c.ToString();
                    c++;
                }
            }
        }

        public static IEnumerable<string> Nicknames
        {
            get
            {
                var i = 1;
                while (true)
                    yield return $"player{i++}";
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}