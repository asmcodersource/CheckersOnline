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
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await Next(context);
                return;
            }
            if (context.Request.Path == "/requestChatSocket")
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var genericWebSocket = new GenericWebSocket(context, webSocket);
                HandleChatRequest(context, genericWebSocket);
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
            cancellationTokenSource.CancelAfter(1000); // max 1000ms for this communication
            var message = await socket.ReceiveMessageAsync(cancellationTokenSource.Token);
            if ( message == null || message.ContainsKey("type") == false || message["type"].ToString() != "connectToChatRoom")
                return;
            int roomId = Convert.ToInt32(message["room_id"]);
            var room = ChatRoomsController.GetRoomById(roomId);
            if (room == null)
                return;
            var client = room.AcceptClientConnect(socket);
            if( client == null ) 
                return;
           // TODO: inform client about ChatClient type
        }
    }
}
