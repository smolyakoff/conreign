using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Utility;

namespace Conreign.Core.Gameplay
{
    public static class Colors
    {
        private static readonly HashSet<string> PopularColors = new HashSet<string>()
        {
            "#ff0000",
            "#008080",
            "#ffe4e1",
            "#0000ff",
            "#d3ffce",
            "#40e0d0",
            "#ffd700",
            "#ff7373",
            "#b0e0e6",
            "#cccccc",
            "#666666",
            "#f6546a",
            "#333333",
            "#003366",
            "#ffa500",
            "#800080",
            "#66cdaa",
            "#800000",
            "#008000",
            "#f08080",
            "#cc0000"
        };

        public static IEnumerable<string> RandomWithPopularFirst()
        {
            foreach (var color in PopularColors.Shuffle())
            {
                yield return color;
            }
            var notPopular = All()
                .Where(x => !PopularColors.Contains(x))
                .Shuffle();
            foreach (var color in notPopular)
            {
                yield return color;
            }
        }

        public static IEnumerable<string> All()
        {
            return Enumerable.Range(0, 0xffffff).Select(x => $"#{x:x}");
        }
    }
}
