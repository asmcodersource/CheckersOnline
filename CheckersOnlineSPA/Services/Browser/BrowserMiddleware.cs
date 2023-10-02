using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace CheckersOnlineSPA.Services.Browser
{
    public class BrowserMiddleware
    {
        RequestDelegate _next;
        BrowserController _controller;
        public BrowserMiddleware(RequestDelegate next, BrowserController browserController)
        {
            _next = next;
            _controller = browserController;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest )
            {
                await _next(context);
                return;
            }
            if (context.Request.Path == "/ws")
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await RegisterWebSocketHandler(context, webSocket);
            }
        }

        public async Task RegisterWebSocketHandler(HttpContext context, WebSocket webSocket)
        {
            var handler = new BrowserSocketHandler(context, webSocket, _controller);
            handler.SocketOpened += (socketHandler) => _controller.AddSocketHandler(socketHandler);
            handler.SocketClosed += (socketHandler) => _controller.RemoveSocketHandler(socketHandler);
            handler.RequestReceived += (socketHandler, requestObject) => _controller.RequestHandler(socketHandler, requestObject);
            await Task.Yield();
            await handler.Handle();
        }
    }
}
