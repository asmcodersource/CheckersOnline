using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersOnlineSPA.CheckersEngine.GameEngine;

namespace CheckersOnlineSPA.CheckersEngine.Controller
{
    public class PlayerController : AbstractController
    {
        public PlayerController( bool isWhite ) : base(isWhite)
        {
            IsWhiteController = isWhite;
        }

        public override async Task<CheckerAction?> GetAction(Game game, bool mustBeat = false)
        {
            do
            {
                Console.WriteLine("(X1, Y1) = ");
                int x1 = Convert.ToInt32(Console.ReadLine());
                int y1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("(X2, Y2) = ");
                int x2 = Convert.ToInt32(Console.ReadLine());
                int y2 = Convert.ToInt32(Console.ReadLine());
                FieldPosition p1 = new FieldPosition(x1, y1);
                FieldPosition p2 = new FieldPosition(x2, y2);
                CheckerAction? action = ActionsGenerator.GetStepAction(p1, p2, game.GameField);
                if (action is not null && action is not WrongAction)
                    return action;
            } while (true);
        }
    }
}
