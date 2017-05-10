using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Utility;

namespace Conreign.Core.AI.Battle
{
    public class NaiveBotBattleStrategy : IBotBattleStrategy
    {
        private readonly PropertyComparer<ReadOnlyPlanetData, string> _planetComparer;
        private readonly Random _random;

        public NaiveBotBattleStrategy()
        {
            _planetComparer = new PropertyComparer<ReadOnlyPlanetData, string>(x => x.Name);
            _random = new Random();
        }

        public List<FleetData> ChooseFleetsToLaunch(Guid userId, ReadOnlyMap map)
        {
            var myPlanets = map.Where(x => x.OwnerId == userId).ToHashSet(_planetComparer);
            var otherPlanets = map.Except(myPlanets, _planetComparer).ToList();
            if (otherPlanets.Count == 0)
            {
                return new List<FleetData>();
            }
            var fleets = myPlanets
                .Select(source => new FleetData
                {
                    From = source.Position,
                    To = otherPlanets[_random.Next(0, otherPlanets.Count)].Position,
                    Ships = source.Ships
                })
                .ToList();
            return fleets;
        }
    }
}
