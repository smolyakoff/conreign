using System;

namespace Conreign.Core.Gameplay.AI
{
    public class NaiveBotBattleStrategyOptions
    {
        public NaiveBotBattleStrategyOptions(double visionFactor, double riskFactor, double clevernessFactor)
        {
            if (visionFactor <= 0 || visionFactor > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(visionFactor), $"Vision factor should be from 0 to 1. Got: {visionFactor}.");
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