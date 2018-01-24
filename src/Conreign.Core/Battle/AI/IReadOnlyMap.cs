using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Core.Battle.AI
{
    public interface IReadOnlyMap : IEnumerable<IPlanetData>
    {
        int MaxDistance { get; }
        int CalculateDistance(IPlanetData from, IPlanetData to);
        int GetPosition(IPlanetData planet);
    }
}