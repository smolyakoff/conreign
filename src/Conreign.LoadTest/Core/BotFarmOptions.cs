using System;

namespace Conreign.LoadTest.Core
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