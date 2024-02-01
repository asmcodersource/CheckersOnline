using CheckersOnlineSPA.Services.Browser;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Games
{
    public class GamesController
    {
        public List<Game> ActiveGames { get; protected set; } = new List<Game>();
        public Dictionary<ClaimsPrincipal, Game> gameByFirstPlayer { get; protected set; } = new Dictionary<ClaimsPrincipal, Game>();
        public Dictionary<ClaimsPrincipal, Game> gameBySecondPlayer { get; protected set; } = new Dictionary<ClaimsPrincipal, Game>();

    
        public int CreateGameRoom(Game game)
        {
            ActiveGames.Add(game);
            gameByFirstPlayer.Add(game.FirstPlayerClaims, game);
            gameBySecondPlayer.Add(game.SecondPlayerClaims, game);
            return ActiveGames.Count - 1;
        }

        public Game? GetUserActiveGame(ClaimsPrincipal user)
        {
            if( gameByFirstPlayer.ContainsKey(user) )
                return gameByFirstPlayer[user];
            else if( gameBySecondPlayer.ContainsKey(user) )
                return gameBySecondPlayer[user];
            else
                return null;
        }
    }
}
