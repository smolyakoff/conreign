using System;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin;
using Orleans;

namespace Conreign.Framework.Http.Owin
{
    internal static class OwinContextExtensions
    {
        public static ILifetimeScope SurelyGetAutofacLifetimeScope(this IOwinContext context)
        {
            var scope = context.GetAutofacLifetimeScope();
            if (scope == null)
            {
                throw new InvalidOperationException("Conreign HTTP framework requires Autofac OWIN middleware to work.");
            }
            return scope;
        }
    }
}