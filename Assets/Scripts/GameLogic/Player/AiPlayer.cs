using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an AI player in the game, implementing the IPlayer interface.
/// </summary>
public class AiPlayer : MonoBehaviour, IPlayer, IPlayerOnInputHandle, IGameLogicInject
{
    public int PlayerId => _playerId;

    public PlayerSymbol PlayerSymbol => _playerSymbol;

    private int _playerId;
    private PlayerSymbol _playerSymbol;
    private IGameLogic _gameLogic;
    private Action<IPlayer, GameBoardPosition> _onSuccessfulInput;

    /// <summary>
    /// Stops the AI player from making a move and blocks priority.
    /// </summary>
    /// <returns>The AI player itself.</returns>
    public IPlayer BlockPriority()
    {
        StopCoroutine(MakeMoveAtEndOfFrame());
        return this;
    }

    /// <summary>
    /// Passes priority to the AI player to make a move.
    /// </summary>
    /// <returns>The AI player itself.</returns>
    public IPlayer PassPriority()
    {
        if (_gameLogic == null)
        {
            Debug.LogError("Cannot make move since game logic has not been set");
            return this;
        }
        StartCoroutine(MakeMoveAtEndOfFrame());
        return this;
    }

    /// <summary>
    /// Sets the player data including player ID and symbol.
    /// </summary>
    /// <param name="playerId">The player's unique identifier.</param>
    /// <param name="playerSymbol">The player's symbol.</param>
    public void SetPlayerData(int playerId, PlayerSymbol playerSymbol)
    {
        _playerId = playerId;
        _playerSymbol = playerSymbol;
    }

    /// <summary>
    /// Sets the callback to be invoked upon a successful input.
    /// </summary>
    /// <param name="onSuccessfulInput">The callback action.</param>
    public void SetCallback(Action<IPlayer, GameBoardPosition> onSuccessfulInput)
    {
        _onSuccessfulInput = onSuccessfulInput;
    }

    /// <summary>
    /// Injects the game logic dependency. Dependency Injector.cs, injects only at awake, therefore additional injection needed to be applied
    /// </summary>
    /// <param name="gameLogic">The game logic to be injected.</param>
    public void InjectGameLogic(IGameLogic gameLogic)
    {
        _gameLogic = gameLogic;
    }

    /// <summary>
    /// Coroutine that makes a move for the AI player at the end of the frame. Its done to prevent double moves happening in one frame
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    private IEnumerator MakeMoveAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        List<(int, int)> freePositions = new();
        var board = _gameLogic.GameBoard.Value;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == 0)
                {
                    freePositions.Add((i, j));
                }
            }
        }
        var randomIndex = UnityEngine.Random.Range(0, freePositions.Count - 1);
        if (randomIndex >= 0)
        {
            _onSuccessfulInput?.Invoke(this, new GameBoardPosition(freePositions[randomIndex].Item1, freePositions[randomIndex].Item2));
        }
    }
}
