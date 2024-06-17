using Auxiliary.Observables;
using DependencyInjection;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Controls the spawning and behavior of players in the game.
/// </summary>
public class PlayersController : MonoBehaviour
{
    [SerializeField] private HumanPlayer playerInputHandlerPrefab;
    [SerializeField] private AiPlayer aiPlayerPrefab;

    [Inject] private IGameLogic _gameLogic;
    [Inject] private IPlayerAssigner _playerAssigner;

    private List<IPlayer> _spawnedPlayers = new List<IPlayer>();
    private Camera _camera;
    private Observer<GameState> _gameStateObserver = new Observer<GameState>();
    private Observer<List<PlayerInfo>> _playerSetupObserver = new Observer<List<PlayerInfo>>();
    private Observer<int[,]> _moveUndoObserver = new Observer<int[,]>();
    private IPlayer currentPlayer;
    private float timeLeft;

    private void Awake()
    {
        _camera = Camera.main;
        _gameStateObserver.AddOnNextListener(val => OnGameStateChange(val)).Observe(_gameLogic.CurrentGameState, false);
        _moveUndoObserver.AddOnNextListener(val => OnMoveUndo(val)).Observe(_gameLogic.OnLastMoveUndo, false);
        _playerSetupObserver.AddOnNextListener(val => SpawnPlayers(val)).Observe(_playerAssigner.CurrentPlayers);
    }

    /// <summary>
    /// Spawns players based on the provided player information.
    /// </summary>
    /// <param name="playersInfo">List of PlayerInfo containing player details.</param>
    private void SpawnPlayers(List<PlayerInfo> playersInfo)
    {
        if (playersInfo == null || playersInfo.Count <= 0)
        {
            Debug.Log($"Players not yet assigned");
            return;
        }

        if (_spawnedPlayers != null && _spawnedPlayers.Count > 1)
        {
            ClearOldPlayers();
        }

        foreach (var playerInfo in playersInfo)
        {
            if (!playerInfo.IsAi)
            {
                var player = Instantiate(playerInputHandlerPrefab);
                _spawnedPlayers.Add(player);
                player.SetCamera(_camera);
                player.SetCallback(OnSuccessfulInput);
                player.SetPlayerData(playerInfo.PlayerId, playerInfo.PlayerSymbol);
            }
            else
            {
                var aiPlayer = Instantiate(aiPlayerPrefab);
                _spawnedPlayers.Add(aiPlayer);
                aiPlayer.SetPlayerData(playerInfo.PlayerId, playerInfo.PlayerSymbol);
                aiPlayer.InjectGameLogic(_gameLogic);
                aiPlayer.SetCallback(OnSuccessfulInput);
            }
        }
        PassPriorityToStartingPlayer();
    }

    /// <summary>
    /// Clears previously spawned players from the scene.
    /// </summary>
    private void ClearOldPlayers()
    {
        foreach (var spawnedPlayer in _spawnedPlayers)
        {
            if (spawnedPlayer is MonoBehaviour mono)
            {
                Destroy(mono.gameObject);
            }
        }
        _spawnedPlayers.Clear();
        _spawnedPlayers.TrimExcess();
    }

    /// <summary>
    /// Handles the change in game state.
    /// </summary>
    /// <param name="gameState">The new GameState value.</param>
    private void OnGameStateChange(GameState gameState)
    {
        if (gameState != GameState.GameStarted)
        {
            foreach (var player in _spawnedPlayers)
            {
                player.BlockPriority();
            }
        }
    }

    /// <summary>
    /// Handles the event when a move is undone.
    /// </summary>
    /// <param name="board">The game board state after the last move undo.</param>
    private void OnMoveUndo(int[,] board)
    {
        bool lastMoveReverted = true;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != 0)
                {
                    lastMoveReverted = false;
                }
            }
        }

        // Find the first player and give priority to them
        if (lastMoveReverted)
        {
            foreach (var val in _spawnedPlayers)
            {
                if (val.PlayerSymbol == PlayerSymbol.Cross)
                {
                    val.PassPriority();
                }
                else
                {
                    val.BlockPriority();
                }
            }
        }
    }

    /// <summary>
    /// Passes priority to the starting player (Cross symbol) among spawned players.
    /// </summary>
    private void PassPriorityToStartingPlayer()
    {
        currentPlayer = _spawnedPlayers.FirstOrDefault(val => val.PlayerSymbol == PlayerSymbol.Cross).PassPriority();
    }

    /// <summary>
    /// Handles the event when an input from a player is successful.
    /// </summary>
    /// <param name="player">The player who made the successful input.</param>
    /// <param name="pos">The position on the game board where the input was made.</param>
    private void OnSuccessfulInput(IPlayer player, GameBoardPosition pos)
    {
        if (_gameLogic.CurrentGameState.Value != GameState.GameStarted)
        {
            return;
        }

        var otherPlayer = _spawnedPlayers.FirstOrDefault(val => val.PlayerId != player.PlayerId);

        if (otherPlayer == null)
        {
            Debug.LogError($"List is populated only with the same player or is null");
            return;
        }

        if (_gameLogic.MakeMove(pos, (int)player.PlayerSymbol))
        {
            Debug.Log($"Priority to {otherPlayer.PlayerSymbol}");
            otherPlayer.PassPriority();
            player.BlockPriority();
        }
        else
        {
            // Player made an invalid move, so pass priority back to them
            player.PassPriority();
        }
    }
}
