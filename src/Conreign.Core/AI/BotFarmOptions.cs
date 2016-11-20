using System;

namespace Conreign.Core.AI
{
    public class BotFarmOptions
    {
        public BotFarmOptions()
        {
            BotStartInterval = TimeSpan.FromSeconds(2);
            GracefulStopPeriod = TimeSpan.FromSeconds(10);
        }

        public TimeSpan BotStartInterval { get; set; }
        public TimeSpan GracefulStopPeriod { get; set; }
    }
}