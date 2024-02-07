using CheckersOnlineSPA.CheckersEngine.Controller;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
using CheckersOnlineSPA.Services.Chat;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Games
{
    public class BotGame : IGame
    {
        public String HumanPlayerEmail { get; set; }
        public GenericWebSocket? HumanPlayerSocket { get; set; }
        public GameState CurrentGameState { get; set; }
        public CheckersEngine.GameEngine.Game? CheckersGame { get; set; }
        public FakeController? HumanFakeController { get; set; }
        public BotController? BotController { get; set; }
        public GamesController GamesController { get; set; }
        public IChatRoom ChatRoom { get; set; }

        public List<GenericWebSocket> ActionsListeners { get; protected set; } = new List<GenericWebSocket>();

        public BotGame(string humanPlayerEmail, GamesController gamesController, ChatRoomsController chatRoomsController)
        {
            this.HumanPlayerEmail = humanPlayerEmail;
            this.GamesController = gamesController;
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

        protected void ConnectPlayer(GenericWebSocket socket, JObject jsonObject)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (HumanPlayerEmail == email)
            {
                ActionsListeners.Add(socket);
                HumanPlayerSocket = socket;
                HumanFakeController = new FakeController(true);
                BotController = new BotController(false, 1);
                CheckersGame = new CheckersEngine.GameEngine.Game(BotController, HumanFakeController);
                CheckersGame.InitializeGame();
                Game.ConnectionEstablishedResponse(socket, this).Wait();
                ChangeState(GameState.WHITE_TURN);
            }
        }

        public void PlayerDisconnected(GenericWebSocket socket)
        {
            ActionsListeners.Remove(socket);
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (HumanPlayerEmail == email)
            {
                HumanPlayerSocket = null;
                GamesController.CloseGameRoom(this);
            }
        }

        protected void CreateGameChat(ChatRoomsController chatRoomsController)
        {
            PublicGameChatRoom room = chatRoomsController.CreateChatRoom(Chat.ChatRoom.ChatRoomType.PublicChatRoom) as PublicGameChatRoom;
            room.ChatRoomRuleABEC.AddAllowedEmail(HumanPlayerEmail);
            this.ChatRoom = room;
        }

        protected async void ProcessPlayerAction(GenericWebSocket socket, JObject jsonObject)
        {
            if (CurrentGameState != GameState.BLACK_TURN && CurrentGameState != GameState.WHITE_TURN)
                return;

            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var playerColor = "white";
            if (CurrentGameState == GameState.WHITE_TURN && playerColor == "white" || CurrentGameState == GameState.BLACK_TURN && playerColor == "black")
            {
                var controller = HumanFakeController;
                var action = Game.ParsePlayerAction(controller, CheckersGame, jsonObject);
                var result = await Game.ExecutePlayerAction(action, CheckersGame);
                await Game.SyncCheckerAction(ActionsListeners, action);
                await PerformBotAction();
            }
        }

        protected async Task PerformBotAction()
        {
            if (CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WaitForNextStep != CheckersGame.LastGameState)
                return;
            ChangeState(CheckersGame.IsWhiteTurn ? GameState.WHITE_TURN : GameState.BLACK_TURN);
            if (CurrentGameState != GameState.BLACK_TURN)
                return;
            var result = await CheckersGame.MakeStep();
            var action = CheckersGame.GetLastAction();
            await Task.Delay(500);
            await Game.SyncCheckerAction(ActionsListeners, action);
            await PerformBotAction();
        }

        protected void ChangeState(GameState newState)
        {
            CurrentGameState = newState;
        }
    }
}
