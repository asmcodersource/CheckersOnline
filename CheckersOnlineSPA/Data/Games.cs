namespace CheckersOnlineSPA.Data
{
    public class Games
    {
        public int Id {get; set;}
        public User FirstPlayer { get; set; }
        public User SecondPlayer { get; set; }
        public GameResult winner {get; set;}

        public Games() {}

        public Games(User firstPlayer, User secondPlayer, GameResult winner)
        {
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            this.winner = winner;
        }
    }
}
