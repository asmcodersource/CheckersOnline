using CheckersOnlineSPA.CheckersEngine.Controller;
using CheckersOnlineSPA.CheckersEngine;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
using Mysqlx.Resultset;
using CheckersOnlineSPA.Services.Chat;

namespace CheckersOnlineSPA.Services.Games
{
    /// <summary>
    /// Represent game room of two humans
    /// Used to handle and perform any requests from players
    /// </summary>
    public class HumansGame : IGame
    {
        public String FirstPlayerEmail { get; set; }
        public GenericWebSocket? FirstPlayerSocket { get; set; }
        public String SecondPlayerEmail { get; set; }
        public GenericWebSocket? SecondPlayerSocket { get; set; }
        public GameState CurrentGameState {  get; set; }
        public CheckersEngine.GameEngine.Game? CheckersGame { get; set; }
        public FakeController? WhiteFakeController { get; set; }
        public FakeController? BlackFakeController { get; set; }
        public GamesController GamesController { get; set; }
        public IChatRoom ChatRoom { get; set; }

        public List<GenericWebSocket> ActionsListeners { get; protected set; } = new List<GenericWebSocket>();

        public HumansGame(ClaimsPrincipal firstPlayer, ClaimsPrincipal secondPlayer, GamesController gamesController, ChatRoomsController chatRoomsController)
        {
            this.FirstPlayerEmail = firstPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            this.SecondPlayerEmail = secondPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            GamesController = gamesController;
            CreateGameChat(chatRoomsController);
        }

        public void ProcessRequest(GenericWebSocket socket, JObject jsonObject)
        {
            try
            {
                switch (jsonObject["type"].ToString())
                {
                    case "connectToRoom":
                        ConnectPlayer(socket, jsonObject);
                        break;
                    case "makeAction":
                        ProcessPlayerAction(socket, jsonObject);
                        break;
                    case "requestChatId":
                        Game.HandleRequestChatId(this, socket);
                        break;
                }
            } catch { }
        }

        public void PlayerDisconnected(GenericWebSocket socket)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            ActionsListeners.Remove(socket);
            if (FirstPlayerEmail == email)
                FirstPlayerSocket = null;
            if (SecondPlayerEmail == email)
                SecondPlayerSocket = null;
            if (SecondPlayerSocket == null && FirstPlayerSocket == null)
                GamesController.CloseGameRoom(this);
        }

        protected void CreateGameChat(ChatRoomsController chatRoomsController)
        {
            PublicGameChatRoom room = chatRoomsController.CreateChatRoom(Chat.ChatRoom.ChatRoomType.PublicChatRoom) as PublicGameChatRoom;
            room.ChatRoomRuleABEC.AddAllowedEmail(FirstPlayerEmail);
            room.ChatRoomRuleABEC.AddAllowedEmail(SecondPlayerEmail);
            this.ChatRoom = room;
        }

        protected void ConnectPlayer(GenericWebSocket socket, JObject jsonObject)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if( email == FirstPlayerEmail || email == SecondPlayerEmail)
            {
                if (email == FirstPlayerEmail)
                    FirstPlayerSocket = socket;
                else
                    SecondPlayerSocket = socket;
                ActionsListeners.Add(socket);
                Game.ConnectionEstablishedResponse(socket, this);
                lock (this)
                {
                    if (FirstPlayerSocket != null && SecondPlayerSocket != null)
                    {
                        WhiteFakeController = new FakeController(true);
                        BlackFakeController = new FakeController(false);
                        CheckersGame = new CheckersEngine.GameEngine.Game(BlackFakeController, WhiteFakeController);
                        CheckersGame.InitializeGame();
                        ChangeState(GameState.WHITE_TURN);
                    }
                }
            } 
        }

        protected async void ProcessPlayerAction(GenericWebSocket socket, JObject jsonObject)
        {
            if (CurrentGameState != GameState.BLACK_TURN && CurrentGameState != GameState.WHITE_TURN)
                return;

            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var playerColor = FirstPlayerEmail == email ? "white" : "black";
            if( CurrentGameState == GameState.WHITE_TURN && playerColor == "white" || CurrentGameState == GameState.BLACK_TURN && playerColor == "black")
            {
                var controller = playerColor == "white" ? WhiteFakeController : BlackFakeController;
                var action = Game.ParsePlayerAction(controller, CheckersGame, jsonObject);
                var result = await Game.ExecutePlayerAction(action, CheckersGame);
                await Game.SyncCheckerAction(ActionsListeners, action);
                if ( CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WaitForNextStep == result)
                    ChangeState(CheckersGame.IsWhiteTurn ? GameState.WHITE_TURN : GameState.BLACK_TURN);
            }
        }

        protected void ChangeState(GameState newState) {
            CurrentGameState = newState;
        }
    }
}
