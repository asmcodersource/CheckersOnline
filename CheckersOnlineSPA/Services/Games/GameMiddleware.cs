using CheckersOnlineSPA.Services.Browser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace CheckersOnlineSPA.Services.Games
{
    public class GameMiddleware
    {
        RequestDelegate _next;
        GamesController _controller;

        public GameMiddleware(RequestDelegate next, GamesController controller)
        {
            _next = next;
            _controller = controller;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }
            if (context.Request.Path == "/requestgamesocket")
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await RegisterWebSocketHandler(context, webSocket);
            } else
            {
                await _next(context);
                return;
            }
        }

        public async Task RegisterWebSocketHandler(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            var handler = new GenericWebSocket(context, webSocket);
            handler.RequestReceived += HandleRequest;

            await Task.Yield();
            await handler.Handle();
        }

        public void HandleRequest(GenericWebSocket socket, JObject requestJsonObject)
        {
            var game = _controller.GetUserActiveGame(socket.User);
            switch(game)
            {
                case HumansGame humansGame:
                    humansGame?.ProcessRequest(socket, requestJsonObject);
                    break;
                case BotGame botGame:
                    botGame?.ProcessRequest(socket, requestJsonObject);
                    break;
            }
            
        }
    }
}
