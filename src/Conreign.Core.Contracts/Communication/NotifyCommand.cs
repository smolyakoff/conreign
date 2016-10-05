using System;
using System.Collections.Generic;

namespace Conreign.Core.Contracts.Communication
{
    public class NotifyCommand
    {
        public HashSet<Guid> UserIds { get; set; }

        public object Event { get; set; }
    }
}