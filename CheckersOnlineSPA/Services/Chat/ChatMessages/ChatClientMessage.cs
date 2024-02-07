namespace CheckersOnlineSPA.Services.Chat.ChatMessages
{
    public class ChatClientMessage
    {
        public String Type { get; } = "ChatClientMessage";
        public String Content { get; set; }
        public ChatClientMessage(String message = null) => Content = message;
    }
}
