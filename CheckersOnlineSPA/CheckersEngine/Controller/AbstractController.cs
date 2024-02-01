using CheckersOnlineSPA.CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.Controller
{
    public class AbstractController
    {
        public bool IsWhiteController { get; protected set; }

        protected AbstractController( bool isWhiteController )
        {
            IsWhiteController = isWhiteController;
        }

        public virtual Task<CheckerAction?> GetAction(Game game, bool mustBeat)
        {
            throw new NotImplementedException();
        }

        public (bool isHaveSteps, bool isHaveBeatSteps) IsControllerHavePossibleStep(GameField gameField )
        {
            bool isHaveSteps = false;
            for (int i = 0; i < 64; i++)
            {
                int x = i % 8;
                int y = i / 8;

                var position = new FieldPosition(x, y);
                var checker = gameField.GetCheckerAtPosition(position);
                if (checker == Checker.None || checker.isWhite() != IsWhiteController)
                    continue;
                var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                if (actions.Count() == 0)
                    continue;
                isHaveSteps = true;
                foreach (CheckerAction action in actions)
                    if (action is BeatByChecker || action is BeatByQueen)
                        return (true, true);
            }
            return (isHaveSteps, false);
        }
    }
}
