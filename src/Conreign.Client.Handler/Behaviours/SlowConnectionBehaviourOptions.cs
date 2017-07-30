using System;

namespace Conreign.Client.Handler.Behaviours
{
    public class SlowConnectionBehaviourOptions
    {
        public SlowConnectionBehaviourOptions() : this(TimeSpan.FromSeconds(3))
        {
        }

        public SlowConnectionBehaviourOptions(TimeSpan delay)
        {
            if (delay.Ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay should be positive.");
            }
            Delay = delay;
        }

        public TimeSpan Delay { get; }
    }
}