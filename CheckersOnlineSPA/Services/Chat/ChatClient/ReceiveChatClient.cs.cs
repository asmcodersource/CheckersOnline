namespace CheckersOnlineSPA.Services.Chat.ChatClient
{
    public class ReceiveChatClient : IChatClient
    {
        public Chat.IChatRoom Room { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GenericWebSocket Socket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action<IChatClient> ClientDisconnected;
        public string Nickname { get; set; }

        public async Task SendMessage(ChatMessages.ChatMessageWrapper message)
        {
            throw new NotImplementedException();
        }
    }
}
