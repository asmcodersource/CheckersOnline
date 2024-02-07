namespace CheckersOnlineSPA.Services.Chat.ChatMessages
{
    [Serializable]
    public class ChatInformationalMessage
    {
        public String Type { get; } = "ChatInformationMessage";
        public String Content { get; set; }
        public ChatInformationalMessage(String message = null) => Content = message;
    }
}
