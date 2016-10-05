using System;

namespace Conreign.Core.Contracts.Communication
{
    public class JoinCommand
    {
        public Guid UserId { get; set; }
        public IObserver Observer { get; set; }
    }
}