﻿using CheckersOnlineSPA.Services.Chat.ChatClient;
using CheckersOnlineSPA.Services.Chat.ChatMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CheckersOnlineSPA.Services.Chat
{
    /// <summary>
    /// This chat client have permission to send and receive messages
    /// </summary>
    public class SendReceiveChatClient: IChatClient
    {
        public event Action<IChatClient> ClientDisconnected;
        public IChatRoom Room { get; set; }
        public GenericWebSocket Socket { get; set; }


        public SendReceiveChatClient(IChatRoom chatRoom, GenericWebSocket socket)
        {
            Room = chatRoom;
            Socket = socket;
            Socket.SocketClosed += ClientSocketCloseHandler;
            Socket.MessageReceived += ClientRequestHandler;
        }


        public async Task SendMessage(ChatMessageWrapper message)
        {
            await Socket?.SendResponseJson(message); 
        }

        protected void ClientRequestHandler(GenericWebSocket socket, JObject request)
        {
            if (request.ContainsKey("type") == false)
                return;

            // TODO: create ChatMessage from request, or handle it another way
            switch (request["type"].ToString())
            {
                case "clientSide":
                    HandleClientRequest(request);
                    break;
                case "roomSide":
                    Room.HandleClientRequest(this, request);
                    break;
            }
            
        }

        protected void HandleClientRequest( JObject request )
        {

        }

        protected void ClientSocketCloseHandler(GenericWebSocket socket)
        {
            Socket = null;
            Room.RemoveClient(this);
        }
    }
}
