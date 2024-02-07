using CheckersOnlineSPA.CheckersEngine.GameEngine;

namespace CheckersOnlineSPA.Services.Chat.ChatClient
{
    /// <summary>
    /// Basic chat client interface
    /// Chat clients may differ in terms of access rights to the chat, rules for processing requests from the client, or the game room.
    /// </summary>
    public interface IChatClient
    {
        public event Action<IChatClient> ClientDisconnected;
        public Chat.IChatRoom Room { get; set; }
        public GenericWebSocket Socket { get; set; }
        public Task SendMessage(ChatMessages.ChatMessageWrapper message);
    }
}
