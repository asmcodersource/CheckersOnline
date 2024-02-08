using CheckersOnlineSPA.Services.Chat.ChatClient;

namespace CheckersOnlineSPA.Services.Chat.ChatMessages
{
    public class ChatClientMessage
    {
        public String Type { get; } = "ChatClientMessage";
        public String Content { get; set; }
        public String Sender { get; set; }
        
        public ChatClientMessage(IChatClient client, String message)
        {
            Content = message;
            Sender = client.Nickname;
        }
    }
}
