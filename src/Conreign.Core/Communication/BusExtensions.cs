using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Communication
{
    internal static class BusExtensions
    {
        public static IBus AsBus(this IBusGrain grain)
        {
            if (grain == null)
            {
                throw new ArgumentNullException(nameof(grain));
            }
            return new Bus(grain);
        }
    }
}
