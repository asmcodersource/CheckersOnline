using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    [Serializable]
    [DataContract]
    [JsonDerivedType(typeof(CheckerMoveAction))]
    [JsonDerivedType(typeof(CheckerBeatAction))]
    [KnownType(typeof(CheckerBeatAction))]
    [KnownType(typeof(CheckerMoveAction))]
    public record CheckerAction
    {
        public FieldPosition FieldStartPosition { get; set; }
        public FieldPosition FieldEndPosition { get; set; }
        public bool BecameQueen { get; set; } = false;
        public bool SwapController { get; set; } = true;

        [JsonConstructor]
        public CheckerAction()
        {

        }

        public CheckerAction(FieldPosition start, FieldPosition end)
        {
            FieldStartPosition = start;
            FieldEndPosition = end;
        }

        public virtual bool VerifyAction(GameField gameField)
        {
            throw new NotImplementedException();
        }
    }

    public record WrongAction : CheckerAction
    {
        [JsonConstructor]
        public WrongAction()
        {

        }
        public WrongAction(FieldPosition start, FieldPosition end) : base(start, end) { }
    }

    [Serializable]
    [DataContract]
    [JsonDerivedType(typeof(MoveChecker))]
    [JsonDerivedType(typeof(MoveQueen))]
    [KnownType(typeof(MoveChecker))]
    [KnownType(typeof(MoveQueen))]
    public record CheckerMoveAction : CheckerAction
    {
        [JsonConstructor]
        public CheckerMoveAction() { }
        public CheckerMoveAction(FieldPosition start, FieldPosition end) : base(start, end) { }
    }

    [Serializable]
    public record MoveChecker : CheckerMoveAction
    {
        public MoveChecker(FieldPosition start, FieldPosition end) : base(start, end) { }

        public override bool VerifyAction(GameField gameField)
        {
            if (FieldStartPosition.IsCloseStep(FieldEndPosition) == false)
                return false;
            if (gameField.GetCheckerAtPosition(FieldEndPosition) != Checker.None)
                return false;
            var checker = gameField.GetCheckerAtPosition(FieldStartPosition);
            if (FieldPosition.IsDirectionRight(FieldStartPosition, FieldEndPosition, checker) == false)
                return false;
            if (FieldEndPosition.IsBecameQueenPosition(checker.isWhite()))
                BecameQueen = true;
            return true;
        }

    }

    [Serializable]
    public record MoveQueen : CheckerMoveAction
    {
        public MoveQueen(FieldPosition start, FieldPosition end) : base(start, end) { }

        public override bool VerifyAction(GameField gameField)
        {
            var checkersOnLine = gameField.GetCheckersBetweenPositions(FieldStartPosition, FieldEndPosition);
            return checkersOnLine.Count == 0;
        }
    }
    [Serializable]
    [DataContract]
    [JsonDerivedType(typeof(BeatByChecker))]
    [JsonDerivedType(typeof(BeatByQueen))]
    [KnownType(typeof(BeatByChecker))]
    [KnownType(typeof(BeatByQueen))]
    public record CheckerBeatAction : CheckerAction
    {

        public FieldPosition CheckerRemovePosition { get; set; }
        public Checker RemoveCheckerType { get; set; }
        [JsonConstructor]
        public CheckerBeatAction(){ }
        public CheckerBeatAction(FieldPosition start, FieldPosition end) : base(start, end) { }
        public virtual bool ShortVerifyAction(GameField gameField)
        {
            throw new NotImplementedException();
        }

        public bool IsControllerShouldBeSwaped(GameField gameField)
        {
            var beatingChecker = gameField.GetCheckerAtPosition(FieldStartPosition);
            var beatChecker = gameField.GetCheckerAtPosition(CheckerRemovePosition);
            gameField.SetCheckerAtPosition(FieldEndPosition, beatingChecker);
            gameField.SetCheckerAtPosition(CheckerRemovePosition, Checker.None);
            var actions = ActionsGenerator.GetCheckerActions(FieldEndPosition, gameField, true);
            bool result = true;
            foreach (var action in actions)
            {
                if (action is CheckerBeatAction)
                {
                    result = false;
                    break;
                }
            }
            gameField.SetCheckerAtPosition(CheckerRemovePosition, beatChecker);
            gameField.SetCheckerAtPosition(FieldEndPosition, Checker.None);
            return result;
        }
    }
    [Serializable]
    public record BeatByChecker : CheckerBeatAction
    {
        public BeatByChecker(FieldPosition start, FieldPosition end) : base(start, end) { }

        public override bool VerifyAction(GameField gameField)
        {
            if( ShortVerifyAction(gameField) == false )
                return false;

            var beatingChecker = gameField.GetCheckerAtPosition(FieldStartPosition);
            if (FieldEndPosition.IsBecameQueenPosition(beatingChecker.isWhite()))
                BecameQueen = true;
            SwapController = IsControllerShouldBeSwaped(gameField);
            return true;
        }

        public override bool ShortVerifyAction(GameField gameField)
        {
            var dx = FieldEndPosition.X - FieldStartPosition.X;
            var dy = FieldEndPosition.Y - FieldStartPosition.Y;
            if (Math.Abs(dy) != 2 && Math.Abs(dx) != 2)
                return false;
            dx = dx > 0 ? 1 : -1;
            dy = dy > 0 ? 1 : -1;
            CheckerRemovePosition = new FieldPosition(FieldStartPosition.X + dx, FieldStartPosition.Y + dy);
            var removeChecker = gameField.GetCheckerAtPosition(CheckerRemovePosition);
            RemoveCheckerType = removeChecker;
            var beatingChecker = gameField.GetCheckerAtPosition(FieldStartPosition);
            if (removeChecker.isWhite() == beatingChecker.isWhite() || removeChecker == Checker.None)
                return false;
            if (gameField.GetCheckerAtPosition(FieldEndPosition) != Checker.None)
                return false;
            return true;
        }

    }

    [Serializable]
    public record BeatByQueen : CheckerBeatAction
    {
        public BeatByQueen(FieldPosition start, FieldPosition end) : base(start, end) { }
        public List<FieldPosition> checkersOnLine;

        public override bool VerifyAction(GameField gameField)
        {
            checkersOnLine = gameField.GetCheckersBetweenPositions(FieldStartPosition, FieldEndPosition);
            if (ShortVerifyAction(gameField) == false)
                return false;
            var beatenPos = checkersOnLine.First();
            var beatenChecker = gameField.GetCheckerAtPosition(beatenPos);
            CheckerRemovePosition = beatenPos;
            RemoveCheckerType = beatenChecker;
            SwapController = IsControllerShouldBeSwaped(gameField);
            return true;
        }

        public override bool ShortVerifyAction(GameField gameField)
        {
            if( checkersOnLine == null )
                checkersOnLine = gameField.GetCheckersBetweenPositions(FieldStartPosition, FieldEndPosition);
            if ( checkersOnLine.Count == 0 ) return false;
            var beatenPos = checkersOnLine.First();
            var beatenChecker = gameField.GetCheckerAtPosition(beatenPos);
            var beatingChecker = gameField.GetCheckerAtPosition(FieldStartPosition);
            if (checkersOnLine.Count != 1)
                return false;
            if (gameField.GetCheckerAtPosition(FieldEndPosition) != Checker.None)
                return false;
            if ((beatenChecker.isWhite() != beatingChecker.isWhite()) == false )
                return false;
            return true;
        }
    }
}
