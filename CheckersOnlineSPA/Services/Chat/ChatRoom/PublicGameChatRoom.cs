using Newtonsoft.Json.Linq;
using CheckersOnlineSPA.Services.Chat.ChatClient;
using CheckersOnlineSPA.Services.Chat.ChatRoomAcceptRules;
using CheckersOnlineSPA.Services.Chat.ChatRoom;
using CheckersOnlineSPA.Services.Chat.ChatMessages;

namespace CheckersOnlineSPA.Services.Chat
{
    public class PublicGameChatRoom : IChatRoom
    {
        public event Action<IChatClient> ClientConnected;
        public event Action<IChatClient> ClientDisconnected;
        public ChatRoomType RoomType { get; protected set; } = ChatRoomType.PublicChatRoom;
        public int Id { get; protected set; }
        protected List<IChatClient> Clients { get; set; }
        public ChatRoomRuleABEC ChatRoomRuleABEC { get; protected set; }

        public PublicGameChatRoom( int roomId ) 
        {
            Id = roomId;
            Clients = new List<IChatClient>();
            ChatRoomRuleABEC = new ChatRoomRuleABEC(this);
        }

        public async Task SendToAnyone(ChatMessages.ChatMessageWrapper message)
        {
            foreach(var client in Clients)
                await client.SendMessage(message);
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

        public void HandleClientRequest(IChatClient? client, JObject request)
        {
            switch (request["action"].ToString())
            {
                case "broadcastMessage":
                    var chatClientMessage = new ChatClientMessage(client, request["content"]["message"].ToString());
                    var chatMessage = new ChatMessages.ChatMessageWrapper(chatClientMessage);
                    SendToAnyone(chatMessage);
                    break;
            }
        }

        public int GetRoomID()
        {
            return Id;
        }
    }
}
