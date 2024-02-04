using Newtonsoft.Json.Linq;

namespace CheckersOnlineSPA.Services.Chat
{
    public class ChatClient
    {
        public ChatRoom Room { get; set; }
        public GenericWebSocket Socket { get; set; }
    
        public ChatClient(ChatRoom chatRoom, GenericWebSocket socket)
        {
            Room = chatRoom;
            Socket = socket;
            Socket.SocketClosed += ClientSocketCloseHandler;
            Socket.MessageReceived += ClientRequestHandler; 
        }


        public void SendMessage(ChatMessage message)
        {

        }

        protected void ClientRequestHandler(GenericWebSocket socket, JObject request)
        {
            if (request.ContainsKey("type") == false)
                return;

            // TODO: create ChatMessage from request, or handle it another way
            switch (request["type"].ToString())
            {
                case "message":
                    break;
            }
            
        }

        protected void ClientSocketCloseHandler(GenericWebSocket socket)
        {
            Room.RemoveClient(this);
        }
    }
}
