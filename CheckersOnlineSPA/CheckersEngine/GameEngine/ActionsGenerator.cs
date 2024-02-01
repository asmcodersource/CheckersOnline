using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    public static class ActionsGenerator
    {
        public static List<CheckerAction> GetCheckerActions(FieldPosition position, GameField gameField, bool callShortVerify = false)
        {
            var actions = new List<CheckerAction>();
            var steps = position.GetAllPossibleSteps();
            foreach (var step in steps)
            {
                var action = GetStepAction(position, step, gameField, callShortVerify);
                if (action is not null && action is not WrongAction)
                    actions.Add(action);
            }
            return actions;
        }

        public static CheckerAction GetStepAction(FieldPosition startPos, FieldPosition endPos, GameField gameField, bool callShortVerify = false )
        {
            CheckerAction action = new WrongAction(startPos, endPos);
            Checker checker = gameField.GetCheckerAtPosition(startPos);
            bool isQueen = checker.isQueen();
            if (isQueen)
            {
                var moveAction = new MoveQueen(startPos, endPos);
                var beatAction = new BeatByQueen(startPos, endPos);
                if (callShortVerify == false)
                {
                    if (moveAction.VerifyAction(gameField))
                        action = moveAction;
                    if (beatAction.VerifyAction(gameField))
                        action = beatAction;
                } else
                {
                    if (moveAction.VerifyAction(gameField))
                        action = moveAction;
                    if (beatAction.ShortVerifyAction(gameField))
                        action = beatAction;
                }
            }
            else
            {
                var moveAction = new MoveChecker(startPos, endPos);
                var beatAction = new BeatByChecker(startPos, endPos);
                if (callShortVerify == false) {
                    if (moveAction.VerifyAction(gameField))
                        action = moveAction;
                    if (beatAction.ShortVerifyAction(gameField))
                        action = beatAction;
                } else
                {
                    if (moveAction.VerifyAction(gameField))
                        action = moveAction;
                    if (beatAction.ShortVerifyAction(gameField))
                        action = beatAction;
                }
            }
            return action;
        }
    }
}
