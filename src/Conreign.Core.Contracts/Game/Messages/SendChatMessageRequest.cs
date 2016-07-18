namespace Conreign.Core.Contracts.Game.Messages
{
    public class SendChatMessageRequest
    {
        public string ChatId { get; set; }
        public string Text { get; set; }
    }
}