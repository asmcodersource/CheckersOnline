using CheckersOnlineSPA.CheckersEngine.BotCore;
using CheckersOnlineSPA.CheckersEngine.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersOnlineSPA.CheckersEngine.GameEngine
{
    public enum GameState
    {
        WaitForNextStep,
        NoMoreStepsLeft,
        WrongActionProvided,
        WrongActionMustBeat,
    }

    public class Game : ICloneable
    {
        public AbstractController BlackController { get; protected set; }
        public AbstractController WhiteController { get; protected set; }
        public ActionsExecutor ActionsExecutor { get; protected set; }
        public bool IsWhiteTurn { get; protected set; }
        public GameField GameField { get; protected set; }
        public AbstractScoreStorage ScoreStorage { get; protected set; }

        public Game(AbstractController blackController, AbstractController whiteController ) { 
            BlackController = blackController;
            WhiteController = whiteController;
            
        }

        public void InitializeGame()
        {
            IsWhiteTurn = true;
            GameField = new GameField();
            GameField.InitializeField();
            ScoreStorage = new ScoreStorage();
            //((ScoreStorage)(ScoreStorage)).LoadFromDatabase();
            ActionsExecutor = new ActionsExecutor(GameField, ScoreStorage);
            ActionsExecutor.RecountCheckersCount();
        }

        public async Task<GameState> MakeStep()
        {
            var controller = IsWhiteTurn ? WhiteController : BlackController;
            var (isHaveSteps, isHaveBeatSteps) = controller.IsControllerHavePossibleStep(GameField);
            if (isHaveSteps == false)
                return GameState.NoMoreStepsLeft;
            var action = await controller.GetAction(this, isHaveBeatSteps);
            if( isHaveBeatSteps )
            {
                // Beating steps must be executed first
                if (action is CheckerMoveAction)
                    return GameState.WrongActionMustBeat;
            }
            if( action == null )
                throw new NullReferenceException("Game controller actions is null");
            if (action.VerifyAction(GameField) == false)
            {
                return GameState.WrongActionProvided;
            }
            if (ActionsExecutor.ExecuteAction(action))
                IsWhiteTurn = !IsWhiteTurn;
            return GameState.WaitForNextStep;
        }

        public void ChangeController(AbstractController controller, bool changeWhiteController )
        {
            if( changeWhiteController )
                WhiteController = controller;
            else 
                BlackController = controller;
        }
        
        public object Clone()
        {
            Game game = new Game(BlackController, WhiteController);
            game.IsWhiteTurn = IsWhiteTurn;
            game.GameField = (GameField)GameField.Clone();
            game.ActionsExecutor = new ActionsExecutor(game.GameField);
            game.ScoreStorage = ScoreStorage;
            return game;
        }

        public void SwapController()
        {
            IsWhiteTurn = !IsWhiteTurn;
        }
    }
}
