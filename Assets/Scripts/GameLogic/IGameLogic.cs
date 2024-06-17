using Auxiliary.Observables;

public interface IGameLogic
{
    public void ResetGame();
    public bool IsMoveLegal(GameBoardPosition placementPosition);
    public bool MakeMove(GameBoardPosition placementPosition, int playerId);
    public void UndoLastTwoMoves();
    public GameBoardPosition GetHintMove();
    public ObservableProperty<GameState> CurrentGameState { get; }
    public ObservableProperty<int[,]> GameBoard { get; }
    public ObservableProperty<int[,]> OnLastMoveUndo { get; }

}

public enum GameState
{
    GameStarted,
    CrossPlayerWon,
    CirclePlayerWon,
    Draw,
    GameAwaiting
}
