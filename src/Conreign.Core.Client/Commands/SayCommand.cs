using System;

namespace Conreign.Core.Client.Commands
{
    public class SayCommand
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; }
        public string Text { get; set; }
    }
}