using Newtonsoft.Json.Linq;

namespace CheckersOnlineSPA.Services.Chat
{
    public class ChatRoom
    {
        public int Id { get; protected set; }
        protected List<ChatClient> Clients { get; set; }

        public ChatRoom( int roomId ) 
        {
            Id = roomId;
            Clients = new List<ChatClient>();
        }

        public void SendMessageToAnyone(ChatMessage message, ChatClient sender)
        {
            ChatClient[] chatClients = Clients.ToArray();
            foreach (var client in Clients)
            {
                if (client != sender)
                    client.SendMessage(message);
            }
        }

        protected void ClientConnected(ChatClient chatClient)
        {

        }

        protected void ClientDisconnected(ChatClient chatClient)
        {
            
        }

        public void AddClient(ChatClient chatClient)
        {
            Clients.Add(chatClient);
            ClientConnected(chatClient);
        }

        public void RemoveClient(ChatClient chatClient)
        {
            ClientDisconnected(chatClient);
            Clients.Remove(chatClient);
        }
    }
}
