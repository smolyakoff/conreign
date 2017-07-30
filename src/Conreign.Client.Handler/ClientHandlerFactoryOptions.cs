using System;
using System.Collections.Generic;
using Conreign.Client.Handler.Behaviours;

namespace Conreign.Client.Handler
{
    public class ClientHandlerFactoryOptions
    {
        public ClientHandlerFactoryOptions()
        {
            Behaviours = new List<Type>
            {
                typeof(DiagnosticsBehaviour<,>),
                typeof(ErrorLoggingBehaviour<,>),
                typeof(AuthenticationBehaviour<,>)
            };
        }

        public List<Type> Behaviours { get; }
    }
}