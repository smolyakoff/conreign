using System;

namespace Conreign.Core.Battle.AI
{
    public class RankingBotBattleStrategyOptions
    {
        public static readonly RankingBotBattleStrategyOptions Default =
            new RankingBotBattleStrategyOptions(0.8, 0.2, 1);

        public RankingBotBattleStrategyOptions(double visionFactor, double riskFactor, double clevernessFactor)
        {
            if (visionFactor <= 0 || visionFactor > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(visionFactor),
                    $"Vision factor should be from 0 to 1. Got: {visionFactor}.");
            }
            if (riskFactor <= 0 || riskFactor > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(riskFactor));
            }
            VisionFactor = visionFactor;
            RiskFactor = riskFactor;
            ClevernessFactor = clevernessFactor;
        }

        public double VisionFactor { get; }

        public double ClevernessFactor { get; }

        public double RiskFactor { get; }
    }
}