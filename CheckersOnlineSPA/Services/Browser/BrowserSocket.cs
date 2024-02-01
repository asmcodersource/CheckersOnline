using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CheckersOnlineSPA.Data;

namespace CheckersOnlineSPA.Services.Browser
{
    public class BrowserSocket
    {
        public event Action<BrowserSocket> SocketOpened;
        public event Action<BrowserSocket> SocketClosed;
        public event Action<BrowserSocket, Newtonsoft.Json.Linq.JObject> RequestReceived;
        public BrowserController BrowserController { get; set; }
        public ClaimsPrincipal User { get; set; }
        WebSocket _socket;

        public BrowserSocket(HttpContext context, WebSocket socket, BrowserController browserController)
        {
            User = GetUser(context);
            _socket = socket;
            BrowserController = browserController;
        }

        public ClaimsPrincipal GetUser(HttpContext context)
        {
            try
            {
                var token = context.Request.Query["token"][0];
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = CheckersOnlineSPA.ClientApp.Auth.AuthOptions.GetSymmetricSecurityKey(), // Replace with your key
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            } catch (Exception ex)
            {
                return null;
            }
        }

        public async Task SendResponseJson(object obj)
        {
            try
            {
                var objJson = System.Text.Json.JsonSerializer.Serialize(obj);
                var jsonBytes = Encoding.UTF8.GetBytes(objJson);
                await _socket.SendAsync(jsonBytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException exception) { }
        }

        public async Task Handle()
        {
            try
            {
                SocketOpened?.Invoke(this);
                while (true)
                {
                    ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[8192]);
                    await _socket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                    await RequestHandler(receiveBuffer);
                }
            } 
            catch (WebSocketException exception){}
            finally
            {
                SocketClosed?.Invoke(this);
            }
        }

        protected async Task RequestHandler(ArraySegment<byte> receiveBuffer)
        {
            var byteArray = receiveBuffer.ToArray();
            var requestString = Encoding.UTF8.GetString(byteArray);
            try
            {
                var jsonObject = JObject.Parse(requestString);
                RequestReceived?.Invoke(this, jsonObject);
            } 
            catch ( JsonReaderException ex ) {}
        }
    }
}
