﻿using System.Net.Sockets;

namespace CheckersOnlineSPA.Services.Chat
{
    public class ChatMiddleware
    {
        protected RequestDelegate Next { get; set; }
        protected ChatRoomsController ChatRoomsController { get; set; }

        public ChatMiddleware(ChatRoomsController chatRoomsController, RequestDelegate next)
        {
            ChatRoomsController = chatRoomsController;
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/requestChatSocket")
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var genericWebSocket = new GenericWebSocket(context, webSocket);
                await HandleChatRequest(context, genericWebSocket);
                await genericWebSocket.Handle();
            }
            else
            {
                await Next(context);
                return;
            }
        }


        public async Task HandleChatRequest(HttpContext context, GenericWebSocket socket)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var message = await socket.ReceiveMessageAsync(cancellationTokenSource.Token);
            if ( message == null || message.ContainsKey("type") == false || message["type"].ToString() != "connectToChatRoom")
                return;
            int chatId = Convert.ToInt32(message["chatId"]);
            var chat = ChatRoomsController.GetRoomById(chatId);
            if (chat == null)
                return;
            var client = chat.AcceptClientConnect(socket);
            if( client == null ) 
                return;

            var response = new
            {
                type = "connectionEstablished",
                clientType = client.GetType().ToString(),
            };
            await socket.SendResponseJson(response);
        }
    }
}
