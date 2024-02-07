using CheckersOnlineSPA.Data;
using CheckersOnlineSPA.Services.Chat;
using CheckersOnlineSPA.Services.Games;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Browser
{
    // Spagetti monkey code... BUT IT WORKS!
    public class BrowserController
    {
        public int LastRoomId { get; protected set; } = 0;
        protected GamesController gamesController;
        protected ChatRoomsController chatRoomsController;
        protected List<GenericWebSocket> socketsHandlers = new List<GenericWebSocket>();
        public Dictionary<string, GameRoom> gameRooms { get; protected set; } = new Dictionary<string, GameRoom>();

        public BrowserController(GamesController gamesController, ChatRoomsController chatRoomsController)
        {
            this.chatRoomsController = chatRoomsController;
            this.gamesController = gamesController;
        }

        public void AddSocket(GenericWebSocket browserSocketHandler)
        {
            socketsHandlers.Add(browserSocketHandler);
        }

        public async Task RemoveSocket(GenericWebSocket browserSocketHandler)
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

        protected GameRoom? CreateRoom(ClaimsPrincipal clientCreator, string title, string description, GenericWebSocket browserSocket)
        {
            GameRoom gameRoom = new GameRoom(LastRoomId, clientCreator, browserSocket, title, description);
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

        protected GameRoom? RemoveRoom(string roomId)
        {
            lock (this)
            {
                if (gameRooms.ContainsKey(roomId))
                {
                    var removedRoom = gameRooms[roomId];
                    gameRooms.Remove(roomId);
                    return removedRoom;
                }
                return null;
            }
        }

        protected async Task<bool> ClaimRoom(string roomId, ClaimsPrincipal claimUser, GenericWebSocket claimSocket)
        {
            try
            {
                if (gamesController.GetUserActiveGame(claimUser) != null)
                    return false; 
                if (gameRooms.ContainsKey(roomId) == false)
                    return false;
                Claim claimId = claimUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var id = claimId.Value;
                if (id == roomId)
                    return false;
                var room = RemoveRoom(roomId);
                SendNotifyRoomRemoved(room);
                var game = new HumansGame(room.ClientCreator, claimUser, gamesController, chatRoomsController);
                gamesController.CreateGameRoom(game);
                // notify that room has claimed for two of players
                await room.CreatorSocket.SendResponseJson(new { type = "claimRoom", state = "roomClaimed" });
                await claimSocket.SendResponseJson(new { type = "claimRoom", state = "roomClaimed" });
                return true;
            } catch {}
            return false;
        }

        protected bool CreateBotRoom(ClaimsPrincipal clientCreator, GenericWebSocket browserSocket)
        {
            string email = clientCreator.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var game = new BotGame(email, gamesController, chatRoomsController);
            gamesController.CreateGameRoom(game);
            browserSocket.SendResponseJson(new { type = "claimRoom", state = "roomClaimed" });
            return true;
        }

        public async Task RequestHandler(GenericWebSocket socketHandler, JObject jsonObject )
        {
            string requestType = jsonObject["type"].ToString();

            if( requestType == "createRoom" && socketHandler.User != null )
            {
                var user = socketHandler.User;
                var title = jsonObject["data"]["title"].ToString();
                var description = jsonObject["data"]["description"].ToString();
                var gameRoom = CreateRoom(user, title, description, socketHandler);
                if( gameRoom != null )
                    await SendNotifyRoomCreated(gameRoom);
            }
            else if (requestType == "createBotRoom" && socketHandler.User != null)
            {
                var user = socketHandler.User;
                var gameRoom = CreateBotRoom(user, socketHandler);
            }
            else if( requestType == "removeRoom" && socketHandler.User != null )
            {
                var user = socketHandler.User;
                var gameRoom = RemoveRoom(user);
                if (gameRoom != null)
                    await SendNotifyRoomRemoved(gameRoom);
            }
            if (requestType == "claimRoom" && socketHandler.User != null)
            {
                var user = socketHandler.User;
                string roomId = Convert.ToString(jsonObject["roomId"]);
                ClaimRoom(roomId, user, socketHandler);
            }
            if ( requestType == "getAllRooms")
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
            var copy = socketsHandlers.ToList<GenericWebSocket>();
            foreach (var handler in copy)
            {
                if( handler != null )
                    tasks.Add(handler.SendResponseJson(sendObject));
            }
            await Task.WhenAll(tasks);
        }
    }
}
