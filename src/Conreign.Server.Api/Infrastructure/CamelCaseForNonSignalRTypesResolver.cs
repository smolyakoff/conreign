using System;
using System.Reflection;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Conreign.Server.Api.Infrastructure
{
    internal class CamelCaseForNonSignalRTypesResolver : IContractResolver
    {
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractResolver;
        private readonly Assembly _signalRAssembly;

        public CamelCaseForNonSignalRTypesResolver()
        {
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            _defaultContractResolver = new DefaultContractResolver();
            _signalRAssembly = typeof(Connection).Assembly;
        }

        public JsonContract ResolveContract(Type type)
        {
            if (type.Assembly.Equals(_signalRAssembly))
            {
                return _defaultContractResolver.ResolveContract(type);
            }
            return _camelCaseContractResolver.ResolveContract(type);
        }
    }
}