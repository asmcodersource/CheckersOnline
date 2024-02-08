using CheckersOnlineSPA.CheckersEngine.GameEngine;
using CheckersOnlineSPA.Services.Chat;
using Newtonsoft.Json.Linq;

namespace CheckersOnlineSPA.Services.Games
{
    public class Game
    {

        public static async Task HandleRequestChatId(IGame game, GenericWebSocket socket)
        {
            var response = new
            {
                type = "ChatIdResponse",
                chatId = game.ChatRoom.GetRoomID(),
            };
            await socket.SendResponseJson(response);
        }

        public static async Task<CheckersEngine.GameEngine.GameState> ExecutePlayerAction(CheckerAction action, CheckersEngine.GameEngine.Game game)
        {
                if (action == null)
                    throw new Exception("Null checker action");
                var result = await game.MakeStep();
                if (result == CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WrongActionMustBeat)
                    throw new Exception("Wrong action provided");
                if (result == CheckersOnlineSPA.CheckersEngine.GameEngine.GameState.WrongActionProvided)
                    throw new Exception("Wrong action provided");
                return result;
        }

        public static CheckerAction ParsePlayerAction(CheckersEngine.Controller.FakeController playerController, CheckersEngine.GameEngine.Game game, JObject jsonObject)
        {
            int x1 = Convert.ToInt32(jsonObject["firstPosition"]["column"]);
            int y1 = Convert.ToInt32(jsonObject["firstPosition"]["row"]);
            int x2 = Convert.ToInt32(jsonObject["secondPosition"]["column"]);
            int y2 = Convert.ToInt32(jsonObject["secondPosition"]["row"]);
            if (Math.Abs(x1 - x2) != Math.Abs(y1 - y2))
                throw new Exception("Non diagonal movement");
            return playerController.GetActionByMove(game, x1, y1, x2, y2);
        }

        public static async Task ConnectionEstablishedResponse(GenericWebSocket socket, IGame game)
        {
            var response = new
            {
                type = "connectionEstablished",
                chatId = game.ChatRoom.GetRoomID()
            };
            await socket.SendResponseJson(response);
        }

        public static async Task SyncCheckerAction(List<GenericWebSocket> clients, CheckerAction checkerAction)
        {
            foreach (var client in clients)
            {
                try
                {
                    switch (checkerAction) {
                        case CheckerMoveAction moveAction:
                            await SyncCheckerMove(client, moveAction);
                            break;
                        case CheckerBeatAction beatAction:
                            await SyncCheckerBeat(client, beatAction);
                            break;
                    }
                } catch { }
            }
            return;
        }


        protected static async Task SyncCheckerMove(GenericWebSocket socket, CheckerAction checkerAction)
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
            await socket.SendResponseJson(moveAction);
        }

        protected static async Task SyncCheckerBeat(GenericWebSocket socket, CheckerBeatAction beatAction)
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
            await socket.SendResponseJson(removeAction);
            await SyncCheckerMove(socket, beatAction);
        }
    }
}
