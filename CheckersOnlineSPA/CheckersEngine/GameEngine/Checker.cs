using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    public enum Checker
    {
        None,
        White,
        Black,
        WhiteQueen,
        BlackQueen
    };

    public static class CheckerExtensions
    {
        public static bool isQueen(this Checker checker) => checker switch
        {
            Checker.BlackQueen => true,
            Checker.WhiteQueen => true,
            _ => false,
        };

        public static bool isWhite(this Checker checker) => checker switch
        {
            Checker.White => true,
            Checker.WhiteQueen => true,
            _ => false,
        };
    }
}
