using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Battle.AI;
using Conreign.Core.Utility;

namespace Conreign.LoadTest.Core
{
    public class NaiveBotBattleStrategy : IBotBattleStrategy
    {
        private readonly PropertyComparer<IPlanetData, string> _planetNameComparer;
        private readonly Random _random;

        public NaiveBotBattleStrategy()
        {
            _planetNameComparer = new PropertyComparer<IPlanetData, string>(x => x.Name);
            _random = new Random();
        }

        public List<FleetData> ChooseFleetsToLaunch(Guid userId, IReadOnlyMap map)
        {
            var myPlanets = map.Where(x => x.OwnerId == userId).ToHashSet(_planetNameComparer);
            var otherPlanets = map.Except(myPlanets, _planetNameComparer).ToList();
            if (otherPlanets.Count == 0)
            {
                return new List<FleetData>();
            }
            var fleets = myPlanets
                .Select(source => new FleetData
                {
                    From = map.GetPosition(source),
                    To = map.GetPosition(otherPlanets[_random.Next(0, otherPlanets.Count)]),
                    Ships = source.Ships
                })
                .ToList();
            return fleets;
        }
    }
}