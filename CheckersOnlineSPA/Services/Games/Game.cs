using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Games
{
    public class Game
    {
        public String FirstPlayerEmail { get; set; }
        public GenericWebSocket? FirstPlayerSocket { get; set; }
        public String SecondPlayerEmail { get; set; }
        public GenericWebSocket? SecondPlayerSocket { get; set; }
        public GameState CurrentGameState {  get; set; }

        public Game(ClaimsPrincipal firstPlayer, ClaimsPrincipal secondPlayer)
        {
            this.FirstPlayerEmail = firstPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            this.SecondPlayerEmail = secondPlayer.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
        }

        public void ProcessRequest(GenericWebSocket socket, JObject jsonObject)
        {
            switch (jsonObject["type"].ToString() )
            {
                case "connectToRoom":
                    ConnectPlayer(socket, jsonObject);        
                    break;
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
