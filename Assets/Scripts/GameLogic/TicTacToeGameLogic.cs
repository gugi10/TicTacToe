using System.Collections.Generic;
using UnityEngine;
using Auxiliary.Observables;

/// <summary>
/// Manages the logic of a tic-tac-toe game, including handling moves, checking game states,
/// and maintaining the game board. Utilizes observable properties to track changes in the game state and board.
/// </summary>
public partial class TicTacToeGameLogic : IGameLogic
{
    /// <summary>
    /// The time in milliseconds for a turn timeout.
    /// </summary>
    private const int TIMEOUT_MILISECONDS = 5000;

    /// <summary>
    /// An observable property representing the current state of the game board.
    /// </summary>
    public ObservableProperty<int[,]> GameBoard { get; } = new();

    /// <summary>
    /// An observable property representing the current state of the game (e.g., awaiting, in progress, won, draw).
    /// </summary>
    public ObservableProperty<GameState> CurrentGameState { get; } = new();

    /// <summary>
    /// An observable property triggered when the last move is undone.
    /// </summary>
    public ObservableProperty<int[,]> OnLastMoveUndo { get; } = new();

    /// <summary>
    /// The size of the game board (3x3).
    /// </summary>
    private const int BOARD_SQUARE_SIZE = 3;

    /// <summary>
    /// A 2D array representing the game board.
    /// </summary>
    private int[,] _gameBoard = new int[BOARD_SQUARE_SIZE, BOARD_SQUARE_SIZE];

    /// <summary>
    /// A stack to keep track of the moves made during the game.
    /// </summary>
    private Stack<GameBoardPosition> _gameHistory = new();

    /// <summary>
    /// An observer for the game board.
    /// </summary>
    private Observer<int[,]> gameboardobserver = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TicTacToeGameLogic"/> class,
    /// setting the initial game state to awaiting and the initial game board state.
    /// </summary>
    public TicTacToeGameLogic()
    {
        CurrentGameState.Set(GameState.GameAwaiting);
        GameBoard.Set(_gameBoard);
    }

    /// <summary>
    /// Checks if a move is legal based on the current state of the game board.
    /// </summary>
    /// <param name="placementPosition">The position to place the move.</param>
    /// <returns>True if the move is legal; otherwise, false.</returns>
    public bool IsMoveLegal(GameBoardPosition placementPosition)
    {
        return placementPosition.columnIndex < _gameBoard.GetLength(1)
            && placementPosition.rowIndex < _gameBoard.GetLength(0)
            && _gameBoard[placementPosition.rowIndex, placementPosition.columnIndex] == 0;
    }

    /// <summary>
    /// Makes a move on the board if it's legal and updates the game state.
    /// Starts a timeout timer and checks for winning conditions or a draw.
    /// </summary>
    /// <param name="placementPosition">The position to place the move.</param>
    /// <param name="playerRepresentation">The player making the move.</param>
    /// <returns>True if the move is successful; otherwise, false.</returns>
    public bool MakeMove(GameBoardPosition placementPosition, int playerRepresentation)
    {
        if (CurrentGameState.Value != GameState.GameStarted)
        {
            Debug.Log($"Current game state is {CurrentGameState.Value}");
            return false;
        }

        if (playerRepresentation > 2 && playerRepresentation < 1)
        {
            Debug.Log($"Wrong player representation used use values in range [1-2]");
            return false;
        }

        if (!IsMoveLegal(placementPosition))
        {
            Debug.Log($"[{placementPosition.rowIndex}] [{placementPosition.columnIndex}]" +
                $"move is illegal");
            return false;

        }
        _gameHistory.Push(placementPosition);
        _gameBoard[placementPosition.rowIndex, placementPosition.columnIndex] = playerRepresentation;
        StaticTimer.StartTimer(TIMEOUT_MILISECONDS, OnTimeout);
        GameBoard.Set(_gameBoard);

        if (IsLastMoveGameWinning(placementPosition, playerRepresentation))
        {
            Debug.Log($"Game is won by {playerRepresentation}");
            CurrentGameState.Set(playerRepresentation == 1 ? GameState.CrossPlayerWon : GameState.CirclePlayerWon);
            return true;
        }
        else if (_gameHistory.Count == BOARD_SQUARE_SIZE * BOARD_SQUARE_SIZE)
        {
            CurrentGameState.Set(GameState.Draw);
            Debug.Log($"Game is a draw");
            return true;
        }

        return true;
    }

    /// <summary>
    /// Undoes the last two moves, updating the game board and triggering the <see cref="OnLastMoveUndo"/> observable property.
    /// </summary>
    public void UndoLastTwoMoves()
    {
        //Undoing two moves when playing vs AI since it does not make sense to undo only ai's move
        for (int i = 0; i < 2; i++)
        {
            if (_gameHistory.Count <= 0)
            {
                OnLastMoveUndo.Set(_gameBoard);
                return;
            }
            var lastMove = _gameHistory.Pop();
            _gameBoard[lastMove.rowIndex, lastMove.columnIndex] = 0;
        }
        OnLastMoveUndo.Set(_gameBoard);
        GameBoard.Set(_gameBoard);
    }

    /// <summary>
    /// Resets the game to its initial state, including resetting the game board and starting a timeout timer.
    /// </summary>
    public void ResetGame()
    {
        CurrentGameState.Set(GameState.GameStarted);
        StaticTimer.StartTimer(TIMEOUT_MILISECONDS, OnTimeout);
        _gameHistory = new();
        for (int i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < _gameBoard.GetLength(1); j++)
            {
                _gameBoard[i, j] = 0;
            }
        }
        GameBoard.Set(_gameBoard);
    }

    /// <summary>
    /// Provides a hint for the next move by finding the first empty spot on the board.
    /// </summary>
    /// <returns>The position of the hint move.</returns>
    public GameBoardPosition GetHintMove()
    {
        for (int i = 0; i < BOARD_SQUARE_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SQUARE_SIZE; j++)
            {
                if (_gameBoard[i, j] == 0)
                {
                    Debug.Log($"{i}{j}");
                    return new GameBoardPosition(i, j);
                }
            }
        }
        return new GameBoardPosition(-1, -1);
    }

    /// <summary>
    /// Handles the timeout event by determining the winner based on the last move made.
    /// </summary>
    private void OnTimeout()
    {
        if (CurrentGameState.Value != GameState.GameStarted)
        {
            return;
        }

        if (_gameHistory.Count == 0)
        {
            CurrentGameState.Set(GameState.CirclePlayerWon);
            return;
        }
        var lastMove = _gameHistory.Pop();
        CurrentGameState.Set(_gameBoard[lastMove.rowIndex, lastMove.columnIndex] == 1 ? GameState.CrossPlayerWon : GameState.CirclePlayerWon);
    }

    /// <summary>
    /// Checks if the last move made by a player results in a win by verifying rows, columns, and diagonals for winning conditions.
    /// </summary>
    /// <param name="lastMovePosition">The position of the last move.</param>
    /// <param name="playerId">The ID of the player making the move.</param>
    /// <returns>True if the last move results in a win; otherwise, false.</returns>
    private bool IsLastMoveGameWinning(GameBoardPosition lastMovePosition, int playerId)
    {
        //Check neighbourhood in row
        for (int i = 0; i < BOARD_SQUARE_SIZE; i++)
        {
            if (_gameBoard[lastMovePosition.rowIndex, i] != playerId)
                break;
            if (i == BOARD_SQUARE_SIZE - 1)
                return true;
        }

        //Check neighbourhood in column
        for (int i = 0; i < BOARD_SQUARE_SIZE; i++)
        {
            if (_gameBoard[i, lastMovePosition.columnIndex] != playerId)
                break;
            if (i == BOARD_SQUARE_SIZE - 1)
                return true;
        }

        //Check neighbourhood in diagonal
        for (int i = 0; i < BOARD_SQUARE_SIZE; i++)
        {
            if (_gameBoard[i, i] != playerId)
                break;
            if (i == BOARD_SQUARE_SIZE - 1)
                return true;
        }

        //Check neighbourhood in anti-diagonal
        for (int i = 0; i < BOARD_SQUARE_SIZE; i++)
        {
            if (_gameBoard[i, BOARD_SQUARE_SIZE - 1 - i] != playerId)
                break;
            if (i == BOARD_SQUARE_SIZE - 1)
                return true;
        }

        return false;
    }
}

/// <summary>
/// Represents a position on the game board with rowIndex and columnIndex properties.
/// </summary>
public struct GameBoardPosition
{
    public int rowIndex; // x - array coordinate 
    public int columnIndex; //y - array coordinate

    public GameBoardPosition(int rowIndex, int columnIndex)
    {
        this.columnIndex = columnIndex;
        this.rowIndex = rowIndex;
    }
}