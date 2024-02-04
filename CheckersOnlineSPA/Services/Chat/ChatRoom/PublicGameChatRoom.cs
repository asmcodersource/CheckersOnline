using Newtonsoft.Json.Linq;
using CheckersOnlineSPA.Services.Chat.ChatClient;
using CheckersOnlineSPA.Services.Chat.ChatRoomAcceptRules;

namespace CheckersOnlineSPA.Services.Chat
{
    public class PublicGameChatRoom : IChatRoom
    {
        public event Action<IChatClient> ClientConnected;
        public event Action<IChatClient> ClientDisconnected;
        public int Id { get; protected set; }
        protected List<IChatClient> Clients { get; set; }
        public ChatRoomRuleABEC ChatRoomRuleABEC { get; protected set; }

        public PublicGameChatRoom( int roomId ) 
        {
            Id = roomId;
            Clients = new List<IChatClient>();
            ChatRoomRuleABEC = new ChatRoomRuleABEC(this);
        }

        public void AddClient(IChatClient chatClient)
        {
            Clients.Add(chatClient);
            ClientConnected?.Invoke(chatClient);
        }

        public void RemoveClient(IChatClient chatClient)
        {
            Clients.Remove(chatClient);
            ClientDisconnected?.Invoke(chatClient);
        }

        public IChatClient? AcceptClientConnect(GenericWebSocket socket)
        {
            var client = ChatRoomRuleABEC.AcceptClient(socket);
            if (client == null)
                return null;
            Clients.Add(client); 
            return client;
        }
    }
}
