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
            if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/requestbrowsersocket")
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
            handler.SocketOpened += (socketHandler) => _controller.AddSocket(socketHandler);
            handler.SocketClosed += (socketHandler) => _controller.RemoveSocket(socketHandler);
            handler.MessageReceived += (socketHandler, requestObject) => _controller.RequestHandler(socketHandler, requestObject);
            await Task.Yield();
            await handler.Handle();
        }
    }
}
