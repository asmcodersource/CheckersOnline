using Azure.Core;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;

namespace CheckersOnlineSPA.Services.OnlineClientsWatcher
{
    /// <summary>
    /// Middleware for counting of active clients per last N minutes
    /// </summary>
    public class OnlineClientsWatcher
    {
        private readonly IDataProtector _protector;
        private readonly RequestDelegate _next;
        private readonly Queue<ClientOnline> clientOnlines = new Queue<ClientOnline>();
        private readonly Dictionary<string, int> tokenCount = new Dictionary<string, int>();
        private readonly TimeOnly ClientOnlineAliveTime = new TimeOnly(0, 1);
        public int ClientsOnline { get => tokenCount.Count; }

        public OnlineClientsWatcher(IDataProtectionProvider protectorProvider)
        {
            _protector = protectorProvider.CreateProtector("userToken");
        }

        public async Task PerformActivity(HttpContext context)
        {
            DequeLateActivities();
            var userToken = context.Request.Cookies["userToken"] ?? AuthenticateUnknown(context);
            if (VerifyToken(userToken) == false)
            {
                context.Response.StatusCode = 403;
                return;
            }
            if (tokenCount.TryAdd(userToken, 1) == false)
                tokenCount[userToken]++;
            var clientActivity = new ClientOnline() { ClientToken = userToken, Time = DateTime.UtcNow };
            clientOnlines.Enqueue(clientActivity);
        }

        public bool VerifyToken(string token)
        {
            try
            {
                _protector.Unprotect(token);
                return true;
            }
            catch { }
            return false;
        }

        public string AuthenticateUnknown(HttpContext context)
        {
            var userToken = _protector.Protect(context.Connection.RemoteIpAddress.ToString());
            context.Response.Cookies.Append("userToken", userToken);
            return userToken;
        }

        public void DequeLateActivities()
        {
            while (clientOnlines.Count() != 0)
            {
                var clientActivity = clientOnlines.Peek();
                var activityTime = clientActivity.Time.AddTicks(ClientOnlineAliveTime.Ticks);
                if (activityTime < DateTime.UtcNow)
                {
                    if (tokenCount[clientActivity.ClientToken] > 1)
                        tokenCount[clientActivity.ClientToken]--;
                    else
                        tokenCount.Remove(clientActivity.ClientToken);
                    clientOnlines.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }
    }

    public class ClientsCounterMiddleware
    {
        RequestDelegate _next;
        public ClientsCounterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, OnlineClientsWatcher clientsCounter)
        {
            await clientsCounter.PerformActivity(context);
            await _next(context);
        }
    }

    record ClientOnline
    {
        public string ClientToken { get; set; } = null!;
        public DateTime Time { get; set; }
    }
}
