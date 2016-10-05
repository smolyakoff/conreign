using System;

namespace Conreign.Core.Contracts.Communication
{
    public class SystemKeys
    {
        public const string StreamProviderName = "Default";
        public static readonly Guid CommunicationStreamKey = Guid.Parse("72ed6d97-a64a-4b0c-bf58-cf0a593301be");
        public const string CommunicationStreamNamespace = "Communication";
    }
}
