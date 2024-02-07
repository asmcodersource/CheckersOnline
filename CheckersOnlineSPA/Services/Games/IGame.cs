using CheckersOnlineSPA.Services.Chat;

namespace CheckersOnlineSPA.Services.Games
{
    /// <summary>
    /// Represent interface for any type of game room
    /// </summary>
    public interface IGame
    {
        public GameState CurrentGameState { get; set; }
        public CheckersEngine.GameEngine.Game? CheckersGame { get; set; }
        public IChatRoom ChatRoom { get; set; }
    }
}
