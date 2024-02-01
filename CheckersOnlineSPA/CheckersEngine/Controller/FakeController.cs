using CheckersOnlineSPA.CheckersEngine;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersOnlineSPA.Services.Games;

namespace CheckersOnlineSPA.CheckersEngine.Controller
{
    public class FakeController : AbstractController
    {
        public CheckerAction ActionToComplete { get; set; }

        public FakeController(bool isWhite) : base(isWhite)
        {
            IsWhiteController = isWhite;
        }

        public override async Task<CheckerAction?> GetAction(CheckersOnlineSPA.CheckersEngine.GameEngine.Game game, bool mustBeat)
        {
            return ActionToComplete;
        }

        public CheckerAction? GetActionByMove(CheckersOnlineSPA.CheckersEngine.GameEngine.Game game, int x1, int y1, int x2, int y2)
        {
            FieldPosition p1 = new FieldPosition(x1, y1);
            FieldPosition p2 = new FieldPosition(x2, y2);
            CheckerAction? action = ActionsGenerator.GetStepAction(p1, p2, game.GameField);
            if (action is not null && action is not WrongAction)
            {
                ActionToComplete = action;
                return action;
            }
            return null;
        }
    }
}
