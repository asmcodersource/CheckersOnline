using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Games
{
    public class Game
    {
        public ClaimsPrincipal FirstPlayerClaims { get; set; }
        public GenericWebSocket? FirstPlayerSocket { get; set; }
        public ClaimsPrincipal SecondPlayerClaims { get; set; }
        public GenericWebSocket? SecondPlayerSocket { get; set; }
        public GameState CurrentGameState {  get; set; }

        public Game(ClaimsPrincipal firstPlayer, ClaimsPrincipal secondPlayer)
        {
            this.FirstPlayerClaims = firstPlayer;
            this.SecondPlayerClaims = secondPlayer;
        }

        public void ProcessRequest(GenericWebSocket socket, JObject jsonObject)
        {
            switch (jsonObject["type"].ToString() )
            {
                case "ConnectToRoom":
                    ConnectPlayer(socket, jsonObject);        
                    break;
            }   
        }

        protected void ConnectPlayer(GenericWebSocket socket, JObject jsonObject)
        {
            if (FirstPlayerClaims == socket.User)
                FirstPlayerSocket = socket;
            if( SecondPlayerClaims == socket.User)
                SecondPlayerSocket = socket;
            if (FirstPlayerSocket != null && SecondPlayerSocket != null)
                ChangeState(GameState.WHITE_TURN);
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
