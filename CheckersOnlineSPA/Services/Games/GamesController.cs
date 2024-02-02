using CheckersOnlineSPA.Data;
using CheckersOnlineSPA.Services.Browser;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Games
{
    public class GamesController
    {
        public List<IGame> ActiveGames { get; protected set; } = new List<IGame>();
        public Dictionary<string, IGame> gameByFirstPlayer { get; protected set; } = new Dictionary<string, IGame>();
        public Dictionary<string, IGame> gameBySecondPlayer { get; protected set; } = new Dictionary<string, IGame>();

    
        public int CreateGameRoom(IGame game)
        {
            ActiveGames.Add(game);
            if (game is HumansGame humansGame)
            {
                gameByFirstPlayer.Add(humansGame.FirstPlayerEmail, game);
                gameBySecondPlayer.Add(humansGame.SecondPlayerEmail, game);
            } else if ( game is BotGame botGame)
            {
                gameByFirstPlayer.Add(botGame.HumanPlayerEmail, game);
            }
            return ActiveGames.Count - 1;
        }

        public void CloseGameRoom(IGame game)
        {
            ActiveGames.Remove(game);
            switch (game)
            {
                case HumansGame humansGame:
                    gameByFirstPlayer.Remove(humansGame.FirstPlayerEmail);
                    gameByFirstPlayer.Remove(humansGame.SecondPlayerEmail);
                    break;
                case BotGame botGame:
                    gameByFirstPlayer.Remove(botGame.HumanPlayerEmail);
                    break;
            };
        }

        public IGame? GetUserActiveGame(ClaimsPrincipal user)
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
