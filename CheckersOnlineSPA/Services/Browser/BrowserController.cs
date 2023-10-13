using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Browser
{
    public class BrowserController
    {
        public int LastRoomId { get; protected set; } = 0;
        protected List<BrowserSocketHandler> socketsHandlers = new List<BrowserSocketHandler>();
        public Dictionary<string, GameRoom> gameRooms { get; protected set; } = new Dictionary<string, GameRoom>();

        public void AddSocketHandler(BrowserSocketHandler browserSocketHandler)
        {
            socketsHandlers.Add(browserSocketHandler);
        }

        public async Task RemoveSocketHandler(BrowserSocketHandler browserSocketHandler)
        {
            socketsHandlers.Remove(browserSocketHandler);
            var user = browserSocketHandler.User;
            if (user != null)
            {
                var gameRoom = RemoveRoom(user);
                if (gameRoom != null)
                    await SendNotifyRoomRemoved(gameRoom);
            }
        }

        protected GameRoom? CreateRoom(ClaimsPrincipal clientCreator, string title, string description)
        {
            GameRoom gameRoom = new GameRoom(LastRoomId, clientCreator, title, description);
            Claim claimId = clientCreator.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var id = claimId.Value;
            if (gameRooms.ContainsKey(id))
                return null;
            gameRooms.Add(id, gameRoom);
            LastRoomId++;
            return gameRoom;
        }

        protected GameRoom? RemoveRoom(ClaimsPrincipal clientCreator)
        {
            lock (this)
            {
                Claim claimId = clientCreator.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var id = claimId.Value;
                if (gameRooms.ContainsKey(id))
                {
                    var removedRoom = gameRooms[id];
                    gameRooms.Remove(id);
                    return removedRoom;
                }
                return null;
            }
        }

        public async Task RequestHandler(BrowserSocketHandler socketHandler, JObject jsonObject )
        {
            string requestType = jsonObject["type"].ToString();

            if( requestType == "createRoom" && socketHandler.User != null )
            {
                var user = socketHandler.User;
                var title = jsonObject["data"]["title"].ToString();
                var description = jsonObject["data"]["description"].ToString();
                var gameRoom = CreateRoom(user, title, description);
                if( gameRoom != null )
                    await SendNotifyRoomCreated(gameRoom);
            } else if( requestType == "removeRoom" && socketHandler.User != null )
            {
                var user = socketHandler.User;
                var gameRoom = RemoveRoom(user);
                if (gameRoom != null)
                    await SendNotifyRoomRemoved(gameRoom);
            } 
            
            if( requestType == "getAllRooms")
            {
                List<object> roomsDTO = new List<object> { };
                foreach( var room in  gameRooms )
                    roomsDTO.Add(room.Value.CreateDTO());
                await socketHandler.SendResponseJson(new { type="allRooms", data=roomsDTO });
            }
        }

        protected async Task SendNotifyRoomCreated(GameRoom room)
        {
            if (room == null)
                return;
            var sendObject = new { type = "addRoom", data = room.CreateDTO() };
            await SendToEveryone(sendObject);
        }

        protected async Task SendNotifyRoomRemoved(GameRoom room)
        {
            if (room == null)
                return;
            var sendObject = new { type = "removeRoom", data = room.CreateDTO() };
            await SendToEveryone(sendObject);
        }

        protected async Task SendToEveryone(object sendObject)
        {
            List<Task> tasks = new List<Task>();
            var copy = socketsHandlers.ToList<BrowserSocketHandler>();
            foreach (var handler in copy)
            {
                if( handler != null )
                    tasks.Add(handler.SendResponseJson(sendObject));
            }
            await Task.WhenAll(tasks);
        }
    }
}
