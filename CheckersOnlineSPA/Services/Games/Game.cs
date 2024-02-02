﻿using CheckersOnlineSPA.CheckersEngine.Controller;
using CheckersOnlineSPA.CheckersEngine;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
using Mysqlx.Resultset;

namespace CheckersOnlineSPA.Services.Games
{
    public class Game
    {
        public String FirstPlayerEmail { get; set; }
        public GenericWebSocket? FirstPlayerSocket { get; set; }
        public String SecondPlayerEmail { get; set; }
        public GenericWebSocket? SecondPlayerSocket { get; set; }
        public GameState CurrentGameState {  get; set; }
        public CheckersEngine.GameEngine.Game? CheckersGame { get; set; }
        public FakeController? WhiteFakeController { get; set; }
        public FakeController? BlackFakeController { get; set; }

        public Game(ClaimsPrincipal firstPlayer, ClaimsPrincipal secondPlayer)
        {
            this.FirstPlayerEmail = firstPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            this.SecondPlayerEmail = secondPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
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

                SynchronizeAction(action);
                if ( CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WaitForNextStep == result)
                {
                    CurrentGameState = CheckersGame.IsWhiteTurn ? GameState.WHITE_TURN : GameState.BLACK_TURN;
                }
            }
        }

        protected void SynchronizeAction(CheckerAction checkerAction)
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
            SendToBothPlayers(moveAction);

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
                SendToBothPlayers(removeAction);
            }
        }

        protected void SendToBothPlayers(object data)
        {
            if (SecondPlayerSocket == null || FirstPlayerSocket == null)
                throw new Exception("At least one of players is not initilized");
            SecondPlayerSocket.SendResponseJson(data);
            FirstPlayerSocket.SendResponseJson(data);
        }

        protected void ChangeState(GameState newState) {
            CurrentGameState = newState;
            
        }
    }
}
