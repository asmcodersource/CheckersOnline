using Newtonsoft.Json.Linq;
using CheckersOnlineSPA.Services.Chat.ChatClient;

namespace CheckersOnlineSPA.Services.Chat
{
    public record ChatMessage
    {
        IChatRoom? Sender { get; set; } // TODO: change to some secured DTO
        DateTime SendTime { get; set; }
        JObject Message { get; set; }
        
        public ChatMessage(JObject Message, IChatRoom? Sender)
        {
            this.Message = Message;
            this.Sender = Sender;
        }
    }   
}
