using CheckersOnlineSPA.CheckersEngine.BotCore;
using CheckersOnlineSPA.CheckersEngine.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    public class ActionsExecutor: ICloneable
    {
        public AbstractScoreStorage ScoreStorage { get; set; }
        public int WhiteCheckersCount { get; protected set; } = 0;
        public int BlackCheckersCount { get; protected set; } = 0;
        public GameField GameField { get; protected set; }
        public List<CheckerAction> ActionsHistory { get; protected set; }


        public ActionsExecutor(GameField gameField)
        {
            GameField = gameField;
            ActionsHistory = new List<CheckerAction>();
            ScoreStorage = new ScoreStorage();
            RecountCheckersCount();
        }

        public ActionsExecutor(GameField gameField, AbstractScoreStorage scoreStorage)
        {
            GameField = gameField;
            ActionsHistory = new List<CheckerAction>();
            ScoreStorage = scoreStorage;
            RecountCheckersCount();
        }

        public void RecountCheckersCount()
        {
            WhiteCheckersCount = 0;
            BlackCheckersCount = 0;
            if (GameField is null)
                throw new NullReferenceException("GameField is null");

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var position = new FieldPosition(x, y);
                    if (GameField.GetCheckerAtPosition(position) == Checker.None)
                        continue;
                    if (GameField.GetCheckerAtPosition(position).isWhite())
                        WhiteCheckersCount++;
                    else
                        BlackCheckersCount++;
                }
            }
        }

        public bool ExecuteAction(CheckerAction action)
        {
            if (action is WrongAction)
                return false;

            var checker = GameField.GetCheckerAtPosition(action.FieldStartPosition);
            GameField.SetCheckerAtPosition(action.FieldEndPosition, checker);
            GameField.SetCheckerAtPosition(action.FieldStartPosition, Checker.None);
            if (action.BecameQueen)
            {
                var queenType = checker.isWhite() ? Checker.WhiteQueen : Checker.BlackQueen;
                GameField.SetCheckerAtPosition(action.FieldEndPosition, queenType);
            }

            if (action is CheckerBeatAction)
            {
                GameField.SetCheckerAtPosition(((CheckerBeatAction)action).CheckerRemovePosition, Checker.None);
                if (checker.isWhite())
                    BlackCheckersCount--;
                else
                    WhiteCheckersCount--;
            }
            ActionsHistory.Add(action);
            return action.SwapController;
        }

        public bool CancelLastAction()
        {
            if (ActionsHistory.Count == 0)
                throw new Exception("Action list is empty");

            var action = ActionsHistory.Last();
            var checker = GameField.GetCheckerAtPosition(action.FieldEndPosition);

            if (action.BecameQueen)
            {
                var checkerType = checker.isWhite() ? Checker.White : Checker.Black;
                checker = checkerType;
            }

            GameField.SetCheckerAtPosition(action.FieldStartPosition, checker);
            GameField.SetCheckerAtPosition(action.FieldEndPosition, Checker.None);

            if (action is CheckerBeatAction)
            {
                var beatAction = (CheckerBeatAction)action;
                GameField.SetCheckerAtPosition(beatAction.CheckerRemovePosition, beatAction.RemoveCheckerType);
                if (checker.isWhite())
                    BlackCheckersCount++;
                else
                    WhiteCheckersCount++;
            }
            ActionsHistory.RemoveAt(ActionsHistory.Count - 1);
            return action.SwapController;
        }

        public object Clone()
        {
            var copiedGameField = (GameField)GameField.Clone();
            var actionsExecutor = new ActionsExecutor(copiedGameField);
            actionsExecutor.BlackCheckersCount = BlackCheckersCount;
            actionsExecutor.WhiteCheckersCount = WhiteCheckersCount;
            actionsExecutor.ActionsHistory = new List<CheckerAction>(ActionsHistory);
            actionsExecutor.ScoreStorage = ScoreStorage;
            return actionsExecutor;
        }
    }
}
