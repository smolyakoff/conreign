using System;

namespace Conreign.Core.Contracts.Communication
{
    public class StreamConstants
    {
        public const string DefaultProviderName = "DefaultStream";
        public static readonly Guid ClientStreamKey = Guid.Parse("72ed6d97-a64a-4b0c-bf58-cf0a593301be");
        public const string ClientStreamNamespace = "ClientStreams";
    }
}
