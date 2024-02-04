using Newtonsoft.Json.Linq;

namespace CheckersOnlineSPA.Services.Chat
{
    public record ChatMessage
    {
        ChatClient? Sender { get; set; }
        JObject Message { get; set; }
        
        public ChatMessage(JObject Message, ChatClient? Sender)
        {
            this.Message = Message;
            this.Sender = Sender;
        }
    }   
}
