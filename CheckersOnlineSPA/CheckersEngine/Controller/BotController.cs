using CheckersOnlineSPA.CheckersEngine.BotCore;
using CheckersOnlineSPA.CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.Controller
{
    public class BotController : AbstractController
    {
        public int Complexity { get; protected set; }
        public double RandomPart { get; protected set; }

        public BotController( bool isWhite, int complexity = 5, double randomPart = 0.0 ) : base(isWhite)
        {
            Complexity = complexity;
            RandomPart = randomPart;
        }

        public override async Task<CheckerAction> GetAction(Game game, bool mustBeat = false)
        {
            var savedAction = game.ScoreStorage.GetResult(game.GameField.GetGameStateIdentify(), IsWhiteController);
            //if( savedAction != null)
            //    return savedAction.FirstCheckerAction;
            FieldScoreProvider simulator = new FieldScoreProvider(game, Complexity);
            await simulator.GetPositionScore(IsWhiteController);
            var results = simulator.Results;
            var bestResult = GetBestResult(results, mustBeat);
            // game.ScoreStorage.StoreResult(game.GameField.GetGameStateIdentify(), bestResult, IsWhiteController, Complexity);
            return bestResult.FirstCheckerAction;
        }

        private FieldScoreResult GetBestResult(List<FieldScoreResult> results, bool mustBeat )
        {
            var array = results.ToArray();
            Array.Sort(array, FieldScoreResult.CompareResults);
            if (array.Length == 0)
                throw new Exception("No actions left exception!");

            for( int i = 0; i < array.Length; i++)
            {
                var result = array[i];
                var action = result.FirstCheckerAction;
                if (action is CheckerBeatAction == false && mustBeat )
                    continue;
                return result;
            }

            throw new Exception("Unexpected failure!");
        }
    }
}