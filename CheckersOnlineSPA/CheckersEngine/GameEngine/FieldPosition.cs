using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    public record FieldPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public FieldPosition(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Return true if FieldPostion values is inside game field.
        /// </summary>
        public bool isInsideGameField()
        {
            if (X < 0 || Y < 0)
                return false;
            if (X > 8 || Y > 8)
                return false;
            return true;
        }

        /// <summary>
        /// Return all posible steps on game field from current position.
        /// It doesn't check checker type, or game field status. 
        /// </summary>
        public List<FieldPosition> GetAllPossibleSteps()
        {
            var steps = new List<FieldPosition>();
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if ((y + x) % 2 == 1 && IsStepPossible(y, x))
                        steps.Add(new FieldPosition(y, x));
            return steps;
        }

        public bool IsStepPossible(int x, int y)
        {
            var fieldPosition = new FieldPosition(x, y);
            if (fieldPosition.isInsideGameField() == false)
                return false;
            if (fieldPosition == this)
                return false;
            if (Math.Abs(x - X) != Math.Abs(y - Y))
                return false;
            return true;
        }

        public FieldPosition GetInvertedPosition()
        {
            return new FieldPosition(X, 7 - Y);
        }

        public bool IsStepPossible(FieldPosition position)
        {
            return IsStepPossible(position.X, position.Y);
        }

        public bool IsCloseStep(FieldPosition secondPos)
        {
            var dx = Math.Abs(secondPos.X - X);
            var dy = Math.Abs(secondPos.Y - Y);
            return dx == 1 && dy == 1;
        }

        public static bool IsDirectionRight(FieldPosition pos1, FieldPosition pos2, Checker checker)
        {
            bool isWhite = checker.isWhite();
            var dy = pos2.Y - pos1.Y > 0 ? 1 : -1;
            if (isWhite && dy != -1)
                return false;
            else if (!isWhite && dy != 1)
                return false;
            return true;
        }

        public bool IsBecameQueenPosition(bool isWhite)
        {
            if (isWhite && Y == 0)
                return true;
            if (!isWhite && Y == 7)
                return true;
            return false;
        }
    }
}
