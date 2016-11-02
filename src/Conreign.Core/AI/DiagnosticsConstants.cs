using System;

namespace Conreign.Core.AI
{
    internal static class DiagnosticsConstants
    {
        public const string BotHandleEventOperationDescription = "Bot.Operations.HandleEvent";
        public const string BotFarmRunOperationDescription = "Bot.Operations.Run";

        public static string BotFarmRunOperationId(string botFarmId)
        {
            return $"Bot.Operations.HandleEvent[{botFarmId}]";
        }

        public static string BotHandleEventOperationId(Type eventType)
        {
            return $"Bot.Operations.HandleEvent[{eventType.Name}]";
        }

        public const int EventsCounterResolution = 10;

        public static string BotReceivedEventsCounterName(string botId)
        {
            return $"Bot.Counters.ReceivedEvents[{botId}]";
        }

        public static string BotProcessedEventsCounterName(string botId)
        {
            return $"Bot.Counters.ProcessedEvents[{botId}]";
        }

        public static string BotQueueSizeGaugeName(string botId)
        {
            return $"Bot.Gauges.QueueLength[{botId}]";
        }
    }
}
