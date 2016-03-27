using System;
using System.Collections.Generic;
using System.Reflection;
using Conreign.Framework.Auth;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using Conreign.Framework.Core;
using Conreign.Framework.Core.Serialization;
using Conreign.Framework.Diagnostics;
using Conreign.Framework.ExceptionHandling;
using Conreign.Framework.Routing;
using MediatR;

namespace Conreign.Framework
{
    public class FrameworkConfiguration
    {
        public FrameworkConfiguration()
        {
            StreamProviderName = "Default";
            Converter = new JsonConverter();
            Pipeline = new List<Type>
            {
                typeof (LoggingDecorator),
                typeof (ExceptionHandlingDecorator),
                typeof (AuthDecorator),
                typeof (Router)
            };
        }

        public string StreamProviderName { get; set; }

        public Assembly GrainContractsAssembly { get; set; }

        public IConverter Converter { get; set; }

        public List<Type> Pipeline { get; set; }

        internal void EnsureIsValid()
        {
            if (GrainContractsAssembly == null)
            {
                throw new InvalidOperationException("Grains assembly is not specified.");
            }
            if (Pipeline == null || Pipeline.Count == 0)
            {
                throw new InvalidOperationException("Pipeline should not be empty.");
            }
            if (!Pipeline.TrueForAll(typeof (IAsyncRequestHandler<Request, Response>).IsAssignableFrom))
            {
                throw new InvalidOperationException(
                    "All pipeline handlers should implement IAsyncHandler<Request, Response>.");
            }
            if (Converter == null)
            {
                throw new InvalidOperationException("Converter is not specified.");
            }
        }
    }
}