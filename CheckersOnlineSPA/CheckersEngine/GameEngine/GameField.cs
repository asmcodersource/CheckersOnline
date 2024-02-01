using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    public class GameField : ICloneable
    {
        public Checker[,]? CheckersField { get; protected set; }

        /// <summary>
        /// Initialize this game field, by creating chekers on game-start position.
        /// Previous game field state will be losted
        /// </summary>
        public void InitializeField()
        {
            // Create game field object
            CheckersField = new Checker[8, 8];

            // Place black chekers
            for (int y = 5; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if ((y + x) % 2 == 1)
                        CheckersField[y, x] = Checker.White;

            // Place While chekers
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 8; x++)
                    if ((y + x) % 2 == 1)
                        CheckersField[y, x] = Checker.Black;
        }

        public void InitializeEmptyField()
        {
            CheckersField = new Checker[8, 8];
        }

        /// <summary>
        /// Make string representation of game field.
        /// Useful for debugging 
        /// </summary>
        public override string ToString()
        {
            if (CheckersField is null)
                return "Game field is null";

            var checkersSymbols = new Dictionary<Checker, string>()
            {
                { Checker.None, "  " },
                { Checker.White, "WC" },
                { Checker.Black, "BC" },
                { Checker.WhiteQueen, "WQ" },
                { Checker.BlackQueen, "BQ" },
            };

            StringBuilder builder = new StringBuilder();
            Console.Write("    ");
            for (int index = 0; index < 8; index++)
                Console.Write($"#{index} ");
            Console.WriteLine();
            for (int y = 0; y < 8; y++)
            {
                builder.Append($"#{y} |");
                for (int x = 0; x < 8; x++)
                {
                    builder.Append(checkersSymbols[CheckersField[y, x]]);
                    builder.Append(' ');
                }
                builder.Append('\n');
            }

            return builder.ToString();
        }

        public Checker GetCheckerAtPosition(int x, int y, bool inverted = false)
        {
            if (CheckersField is null)
                throw new NullReferenceException("Game field is null");
            var fieldPosition = new FieldPosition(x, y);
            if (fieldPosition.isInsideGameField() is not true)
                throw new ArgumentOutOfRangeException("Position is outside of game field");
            y = inverted ? 7 - y : y;
            return CheckersField[y, x];
        }

        public Checker GetCheckerAtPosition(FieldPosition position, bool inverted = false)
        {
            return GetCheckerAtPosition(position.X, position.Y, inverted);
        }

        public List<FieldPosition> GetCheckersBetweenPositions(FieldPosition pos1, FieldPosition pos2)
        {
            var results = new List<FieldPosition>();
            if (pos1.IsStepPossible(pos2) == false)
                return results;
            var dx = pos2.X - pos1.X > 0 ? 1 : -1;
            var dy = pos2.Y - pos1.Y > 0 ? 1 : -1;
            var checkPosition = pos1;
            do
            {
                checkPosition = new FieldPosition(checkPosition.X + dx, checkPosition.Y + dy);
                var checker = GetCheckerAtPosition(checkPosition);
                if (checker != Checker.None)
                    results.Add(checkPosition);
            } while (checkPosition != pos2);
            return results;
        }

        public void SetCheckerAtPosition(int x, int y, Checker checker)
        {
            SetCheckerAtPosition(new FieldPosition(x, y), checker);
        }

        public void SetCheckerAtPosition(FieldPosition fieldPosition, Checker checker)
        {
            if (CheckersField is null)
                throw new NullReferenceException("Game field is null");
            if (fieldPosition.isInsideGameField() is not true)
                throw new ArgumentOutOfRangeException("Position is outside of game field");
            CheckersField[fieldPosition.Y, fieldPosition.X] = checker;
        }

        public object Clone()
        {
            GameField gameField = new GameField();
            gameField.CheckersField = (Checker[,])CheckersField.Clone();
            return gameField;
        }


        public System.Numerics.BigInteger GetGameStateIdentify()
        {
            System.Numerics.BigInteger gameStateIdentify = 0;
            int i = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 != 1)
                        continue;
                    var checker = GetCheckerAtPosition(x, y);
                    var value = (int)checker * System.Numerics.BigInteger.Pow(5, i);
                    gameStateIdentify += value;
                    i++;
                }
            }
            return gameStateIdentify;
        }
    }
}