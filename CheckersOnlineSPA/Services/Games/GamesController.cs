using CheckersOnlineSPA.Data;
using CheckersOnlineSPA.Services.Browser;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Games
{
    public class GamesController
    {
        public List<Game> ActiveGames { get; protected set; } = new List<Game>();
        public Dictionary<string, GenericWebSocket> connections { get; protected set; } = new Dictionary<string, GenericWebSocket>();
        public Dictionary<string, Game> gameByFirstPlayer { get; protected set; } = new Dictionary<string, Game>();
        public Dictionary<string, Game> gameBySecondPlayer { get; protected set; } = new Dictionary<string, Game>();

    
        public int CreateGameRoom(Game game)
        {
            ActiveGames.Add(game);
            gameByFirstPlayer.Add(game.FirstPlayerEmail, game);
            gameBySecondPlayer.Add(game.SecondPlayerEmail, game);
            return ActiveGames.Count - 1;
        }

        public Game? GetUserActiveGame(ClaimsPrincipal user)
        {
            var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if ( gameByFirstPlayer.ContainsKey(email) )
                return gameByFirstPlayer[email];
            else if( gameBySecondPlayer.ContainsKey(email) )
                return gameBySecondPlayer[email];
            else
                return null;
        }
    }
}
