using System;

namespace Conreign.Core.AI
{
    public class BotFarmOptions
    {
        public BotFarmOptions()
        {
            StartupDelay = TimeSpan.FromSeconds(2);
        }

        public TimeSpan StartupDelay { get; set; }
    }
}