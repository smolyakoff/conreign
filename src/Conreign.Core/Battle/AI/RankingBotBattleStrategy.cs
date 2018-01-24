using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Utility;

namespace Conreign.Core.Battle.AI
{
    public class RankingBotBattleStrategy : IBotBattleStrategy
    {
        private readonly RankingBotBattleStrategyOptions _options;
        private readonly PropertyComparer<IPlanetData, string> _planetNameComparer;

        public RankingBotBattleStrategy(RankingBotBattleStrategyOptions options)
        {
            _planetNameComparer = new PropertyComparer<IPlanetData, string>(x => x.Name);
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public List<FleetData> ChooseFleetsToLaunch(Guid userId, IReadOnlyMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            if (!map.Any())
            {
                return new List<FleetData>();
            }
            
            var myPlanets = map
                .Where(x => x.OwnerId == userId)
                .ToHashSet(_planetNameComparer);
            var allPlanets = map
                .ToHashSet(_planetNameComparer);
            if (myPlanets.Count == 0)
            {
                return new List<FleetData>();
            }
            var topPlanet = allPlanets.OrderByDescending(RankPlanet).First();
            var myTopPlanet = myPlanets.OrderByDescending(RankPlanet).First();
            var candidates = myPlanets
                .AsParallel()
                .SelectMany(source =>
                {
                    var otherPlanets = allPlanets
                        .Except(new[] {source}, _planetNameComparer);
                    return GetCandidates(map, source, otherPlanets, myTopPlanet, topPlanet);
                })
                .OrderByDescending(Rank)
                .ToList();
            var sentShips = new Dictionary<string, int>();
            var fleets = new List<FleetData>();
            foreach (var candidate in candidates)
            {
                var sent = sentShips.GetOrCreateDefault(candidate.Source.Name);
                if (candidate.Ships + sent > candidate.Source.Ships)
                {
                    continue;
                }
                fleets.Add(new FleetData
                {
                    From = map.GetPosition(candidate.Source),
                    To = map.GetPosition(candidate.Destination),
                    Ships = candidate.Ships
                });
                sentShips[candidate.Source.Name] += candidate.Ships;
            }
            return fleets;
        }

        private IEnumerable<Candidate> GetCandidates(
            IReadOnlyMap map,
            IPlanetData source,
            IEnumerable<IPlanetData> otherPlanets,
            IPlanetData myTopPlanet,
            IPlanetData topPlanet)
        {
            return otherPlanets
                .Select(destination => new Candidate
                {
                    Source = source,
                    Destination = destination,
                    Ships = ShouldLaunchShips(map, source, destination),
                    MyTopPlanet = myTopPlanet,
                    TopPlanet = topPlanet
                })
                .Where(fleet => fleet.Ships > 0);
        }

        private int ShouldLaunchShips(IReadOnlyMap map, IPlanetData source, IPlanetData destination)
        {
            if (!IsVisible(map, source, destination))
            {
                return 0;
            }
            return source.OwnerId == destination.OwnerId
                ? ShouldLaunchReinforcementShips(source, destination)
                : ShouldLaunchAttackShips(map, source, destination);
        }

        private int ShouldLaunchAttackShips(IReadOnlyMap map, IPlanetData source, IPlanetData destination)
        {
            var distance = map.CalculateDistance(source, destination);
            var enemyExpectedPower = (destination.Ships + destination.ProductionRate * distance) * destination.Power;
            var requiredShips = (int) Math.Ceiling(enemyExpectedPower / source.Power * _options.ClevernessFactor);
            var enoughShips = source.Ships >= requiredShips;
            if (!enoughShips)
            {
                return 0;
            }
            var difference = source.Ships - requiredShips;
            if (difference == 0)
            {
                return requiredShips;
            }
            var addition = (int) Math.Floor(difference - _options.RiskFactor * difference);
            return requiredShips + addition;
        }

        private int ShouldLaunchReinforcementShips(IPlanetData source,
            IPlanetData destination)
        {
            var stupidPower = (1 - source.Power) * (1 - _options.ClevernessFactor);
            var powerRatio = (source.Power + stupidPower) / destination.Power;
            var shouldLaunch = powerRatio < 1;
            if (!shouldLaunch)
            {
                return 0;
            }
            var ships = (int) Math.Floor(source.Ships * _options.RiskFactor);
            return ships;
        }

        private bool IsVisible(IReadOnlyMap map, IPlanetData source, IPlanetData destination)
        {
            var distance = map.CalculateDistance(source, destination);
            return distance < map.MaxDistance * _options.VisionFactor;
        }

        private static double Rank(Candidate candidate)
        {
            return candidate.Source.OwnerId == candidate.Destination.OwnerId
                ? RankReinforcementsFleet(candidate)
                : RankAttackFleet(candidate);
        }

        private static double RankAttackFleet(Candidate candidate)
        {
            var destination = candidate.Destination;
            return RankPlanet(destination) / RankPlanet(candidate.TopPlanet);
        }

        private static double RankReinforcementsFleet(Candidate candidate)
        {
            var destination = candidate.Destination;
            return RankPlanet(destination) / RankPlanet(candidate.MyTopPlanet);
        }

        private static double RankPlanet(IPlanetData planet)
        {
            return planet.Power * planet.ProductionRate;
        }

        private class Candidate
        {
            public int Ships { get; set; }
            public IPlanetData Source { get; set; }
            public IPlanetData Destination { get; set; }
            public IPlanetData MyTopPlanet { get; set; }
            public IPlanetData TopPlanet { get; set; }
        }
    }
}