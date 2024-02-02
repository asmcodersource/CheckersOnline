using CheckersOnlineSPA.CheckersEngine.Controller;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
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

        public BotGame(string humanPlayerEmail, GamesController gamesController)
        {
            this.HumanPlayerEmail = humanPlayerEmail;
            this.GamesController = gamesController;
        }

        public void ProcessRequest(GenericWebSocket socket, JObject jsonObject)
        {
            try
            {
                lock (this)
                {
                    switch (jsonObject["type"].ToString())
                    {
                        case "connectToRoom":
                            ConnectPlayer(socket, jsonObject);
                            break;
                        case "makeAction":
                            ProcessPlayerAction(socket, jsonObject);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void ConnectPlayer(GenericWebSocket socket, JObject jsonObject)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (HumanPlayerEmail == email)
            {
                HumanPlayerSocket = socket;
                HumanFakeController = new FakeController(true);
                BotController = new BotController(false, 7);
                CheckersGame = new CheckersEngine.GameEngine.Game(BotController, HumanFakeController);
                CheckersGame.InitializeGame();
                ChangeState(GameState.WHITE_TURN);
            }
        }

        public void PlayerDisconnected(GenericWebSocket socket)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (HumanPlayerEmail == email)
            {
                HumanPlayerSocket = null;
                GamesController.CloseGameRoom(this);
            }
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
                int x1 = Convert.ToInt32(jsonObject["firstPosition"]["column"]);
                int y1 = Convert.ToInt32(jsonObject["firstPosition"]["row"]);
                int x2 = Convert.ToInt32(jsonObject["secondPosition"]["column"]);
                int y2 = Convert.ToInt32(jsonObject["secondPosition"]["row"]);
                var action = controller.GetActionByMove(CheckersGame, x1, y1, x2, y2);
                if (action == null)
                    return;
                var result = await CheckersGame.MakeStep();
                if (result == CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WrongActionMustBeat)
                    return;
                if (result == CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WrongActionProvided)
                    return;

                await Task.Delay(50);
                await SynchronizeAction(action);
                await PerformBotAction();
            }
        }

        protected async Task PerformBotAction()
        {
            if (CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WaitForNextStep != CheckersGame.LastGameState)
                return;
            CurrentGameState = CheckersGame.IsWhiteTurn ? GameState.WHITE_TURN : GameState.BLACK_TURN;
            if (CurrentGameState != GameState.BLACK_TURN)
                return;
            var result = await CheckersGame.MakeStep();
            var action = CheckersGame.GetLastAction();
            await SynchronizeAction(action);
            await PerformBotAction();
        }

        protected async Task SynchronizeAction(CheckerAction checkerAction)
        {

            var moveAction = new
            {
                type = "moveAction",
                firstPosition = new
                {
                    row = checkerAction.FieldStartPosition.Y,
                    column = checkerAction.FieldStartPosition.X,
                },
                secondPosition = new
                {
                    row = checkerAction.FieldEndPosition.Y,
                    column = checkerAction.FieldEndPosition.X,
                },
            };
            await HumanPlayerSocket.SendResponseJson(moveAction);

            if (checkerAction is CheckerBeatAction beatAction)
            {
                var removeAction = new
                {
                    type = "removeAction",
                    removePosition = new
                    {
                        row = beatAction.CheckerRemovePosition.Y,
                        column = beatAction.CheckerRemovePosition.X,
                    }
                };
                await HumanPlayerSocket.SendResponseJson(removeAction);
            }
        }

        protected void ChangeState(GameState newState)
        {
            CurrentGameState = newState;
        }
    }
}
