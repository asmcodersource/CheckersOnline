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
                HandleCharRequest(context, genericWebSocket);
            }
            else
            {
                await Next(context);
                return;
            }
        }


        public async Task HandleCharRequest(HttpContext context, GenericWebSocket socket)
        {
            // TODO: Invent handle of requests to chat
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(5000); // at least 5 seconds for this communication
            var message = await socket.ReceiveMessageAsync(cancellationTokenSource.Token);
            if ( message == null || message.ContainsKey("type") == false || message["type"].ToString() != "connectToChatRoom")
                return;
            int roomId = Convert.ToInt32(message["room_id"]);
            

        }
    }
}
