using CheckersOnlineSPA.CheckersEngine.Controller;
using CheckersOnlineSPA.CheckersEngine;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
using Mysqlx.Resultset;

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

        public HumansGame(ClaimsPrincipal firstPlayer, ClaimsPrincipal secondPlayer, GamesController gamesController)
        {
            this.FirstPlayerEmail = firstPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            this.SecondPlayerEmail = secondPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            GamesController = gamesController;
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
            } catch (Exception ex)
            {

            }
        }

        protected void ConnectPlayer(GenericWebSocket socket, JObject jsonObject)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (FirstPlayerEmail == email)
                FirstPlayerSocket = socket;
            if( SecondPlayerEmail == email)
                SecondPlayerSocket = socket;
            if (FirstPlayerSocket != null && SecondPlayerSocket != null)
            {
                WhiteFakeController = new FakeController(true);
                BlackFakeController = new FakeController(false);
                CheckersGame = new CheckersEngine.GameEngine.Game(BlackFakeController, WhiteFakeController);
                CheckersGame.InitializeGame();
                ChangeState(GameState.WHITE_TURN);
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

                await SynchronizeAction(action);
                if ( CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WaitForNextStep == result)
                {
                    CurrentGameState = CheckersGame.IsWhiteTurn ? GameState.WHITE_TURN : GameState.BLACK_TURN;
                }
            }
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
            await SendToBothPlayers(moveAction);

            if (checkerAction is CheckerBeatAction beatAction) {
                var removeAction = new
                {
                    type = "removeAction",
                    removePosition = new
                    {
                        row = beatAction.CheckerRemovePosition.Y,
                        column = beatAction.CheckerRemovePosition.X,
                    }
                };
                await SendToBothPlayers(removeAction);
            }
        }

        public void PlayerDisconnected(GenericWebSocket socket)
        {
            var email = socket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (FirstPlayerEmail == email)
                FirstPlayerSocket = null;
            if( SecondPlayerEmail == email) 
                SecondPlayerSocket = null;
            if (SecondPlayerSocket == null || FirstPlayerSocket == null)
                GamesController.CloseGameRoom(this);
        }

        protected async Task SendToBothPlayers(object data)
        {
            if (SecondPlayerSocket == null || FirstPlayerSocket == null)
                throw new Exception("At least one of players is not initilized");
            await SecondPlayerSocket.SendResponseJson(data);
            await FirstPlayerSocket.SendResponseJson(data);
        }

        protected void ChangeState(GameState newState) {
            CurrentGameState = newState;
        }
    }
}
