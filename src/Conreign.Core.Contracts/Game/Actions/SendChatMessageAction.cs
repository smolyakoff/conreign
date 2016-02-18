using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Game.Data;

namespace Conreign.Core.Contracts.Game.Actions
{
    public class SendChatMessageAction : IPayloadContainer<MessagePayload>, IMetadataContainer<IUserMeta>
    {
        public IUserMeta Meta { get; set; }
        public MessagePayload Payload { get; set; }
    }
}