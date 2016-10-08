using System;

namespace Conreign.Core.Client
{
    public class MessageMetadata
    {
        public string AccessToken { get; set; }
        public Guid? UserId { get; set; }
        public string TraceId { get; set; }
    }
}