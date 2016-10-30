using System;

namespace Conreign.Core.AI.LoadTest
{
    public class LoadTestBotOptions
    {
        public LoadTestBotOptions()
        {
            RoomsCount = 1;
            BotsPerRoomCount = 3;
            NeutralPlanetsCount = 8;
            MapWidth = 8;
            MapHeight = 8;
            JoinRoomDelay = TimeSpan.FromSeconds(1);
        }

        public int RoomsCount { get; set; }
        public int BotsPerRoomCount { get; set; }
        public int NeutralPlanetsCount { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public TimeSpan JoinRoomDelay { get; set; }
    }
}