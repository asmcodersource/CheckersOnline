namespace CheckersOnlineSPA.Services.OnlineClientsWatcher
{
    public record ClientsActivity
    {
        public int ClientsOnline { get; set; }
        public int GamesOnline { get; set; }

        public static async Task GetClientsActivityHandler(HttpContext context, OnlineClientsWatcher counter)
        {
            var clientsActivity = new ClientsActivity();
            clientsActivity.ClientsOnline = counter.ClientsOnline;
            clientsActivity.GamesOnline = 0;
            await context.Response.WriteAsJsonAsync(clientsActivity);
        }
    }
}
