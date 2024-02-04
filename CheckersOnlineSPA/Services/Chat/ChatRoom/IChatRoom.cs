using MySqlX.XDevAPI;
using CheckersOnlineSPA.Services.Chat.ChatClient;

namespace CheckersOnlineSPA.Services.Chat
{
    /// <summary>
    /// Basic chat rooms interface
    /// Each chat room represents its own implementation of handling requests, handling client connections or disconnections. In this way functional responsibility is divided, everything related to the game room is done by the game room or its members.
    /// </summary>
    public interface IChatRoom
    {
        public IChatClient? AcceptClientConnect(GenericWebSocket socket);
        public void AddClient(IChatClient chatClient);
        public void RemoveClient(IChatClient chatClient);

        public event Action<IChatClient> ClientConnected;
        public event Action<IChatClient> ClientDisconnected;
    }
}
