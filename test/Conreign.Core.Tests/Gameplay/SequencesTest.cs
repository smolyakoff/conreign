using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Editor;
using Xunit;

namespace Conreign.Core.Tests.Gameplay
{
    public class SequencesTest
    {
        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(1000)]
        public void PlanetNames__Generates_Sequence_With_Increasing_Name_Length(int count)
        {
            var planets = Sequences.PlanetNames.Take(count).ToList();

            var sorted = planets.OrderBy(x => x.Length).ToList();
            Assert.Equal(sorted, planets);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(300)]
        public void Colors__Random_With_Popular_First__Each_Element_Is_Unique(int count)
        {
            var colors = Sequences.RandomWithPopularFirstColors.Take(count).ToList();
            var set = new HashSet<string>(colors);

            Assert.Equal(colors.Count, set.Count);
        }

        [Fact]
        public void Colors__Random_With_Popular_First__Each_Element_Has_7_Characters()
        {
            const int length = 50;
            var colors = Sequences.RandomWithPopularFirstColors.Take(length).ToList();

            Assert.All(colors, x => Assert.True(x.Length == 7));
        }
    }
}