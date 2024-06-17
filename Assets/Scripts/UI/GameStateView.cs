using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DependencyInjection;
using Auxiliary.Observables;
using System.Linq;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Monitors and manages the game state, displaying appropriate UI panels and handling player interactions.
    /// </summary>
    public class GameStateView : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;

        [Header("UI elements")]
        [SerializeField] private GameEndPanel gameEndPanel;
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button hintButton;

        [Inject] private IGameLogic _gamelogic;
        [Inject] private IPlayerAssigner _playerAssigner;

        private Observer<GameState> _gameStateObserver = new Observer<GameState>();
        private MainMenuPanel spawnedMainMenu;

        /// <summary>
        /// Initializes the observers for game state changes and button click events.
        /// </summary>
        private void Awake()
        {
            _gameStateObserver.AddOnNextListener(val => OnGameStateChange(val)).Observe(_gamelogic.CurrentGameState);
            undoButton.onClick.AddListener(OnUndoButton);
            hintButton.onClick.AddListener(OnHintButton);
        }

        /// <summary>
        /// Handles changes in game state and triggers corresponding UI updates or actions.
        /// </summary>
        /// <param name="gameState">The new game state.</param>
        private void OnGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.GameAwaiting:
                    ShowMainMenu();
                    break;
                case GameState.CirclePlayerWon:
                case GameState.CrossPlayerWon:
                case GameState.Draw:
                    ShowGameEndPanel(gameState);
                    break;
                default:
                    Debug.LogWarning($"Unhandled game state change: {gameState}");
                    break;
            }
        }

        /// <summary>
        /// Displays the game end panel based on the specified game state.
        /// </summary>
        /// <param name="gameState">The game state indicating the end condition.</param>
        private void ShowGameEndPanel(GameState gameState)
        {
            var spawnedPanel = Instantiate(gameEndPanel);
            int winningPlayerId = GetWinningPlayerId(gameState);
            spawnedPanel.Setup(gameState, winningPlayerId, OnRestart);
        }

        /// <summary>
        /// Retrieves the ID of the winning player based on the game state.
        /// </summary>
        /// <param name="gameState">The game state indicating the winner or draw.</param>
        /// <returns>The ID of the winning player or 0 for a draw.</returns>
        private int GetWinningPlayerId(GameState gameState)
        {
            var players = _playerAssigner?.CurrentPlayers?.Value;
            if (players == null)
                return 0; // No players assigned

            switch (gameState)
            {
                case GameState.CirclePlayerWon:
                    return players.FirstOrDefault(p => p.PlayerSymbol == PlayerSymbol.Circle).PlayerId;
                case GameState.CrossPlayerWon:
                    return players.FirstOrDefault(p => p.PlayerSymbol == PlayerSymbol.Cross).PlayerId;
                case GameState.Draw:
                    return 0; // Draw has no winning player
                default:
                    return 0; // Invalid state
            }
        }

        /// <summary>
        /// Restarts the game when invoked by the game end panel.
        /// </summary>
        /// <param name="spawnedPanel">The game end panel instance.</param>
        private void OnRestart(GameEndPanel spawnedPanel)
        {
            Destroy(spawnedPanel.gameObject);
            _gamelogic.CurrentGameState.Set(GameState.GameAwaiting);
        }

        /// <summary>
        /// Shows the main menu panel and sets up event handlers for local multiplayer or AI game modes.
        /// </summary>
        private void ShowMainMenu()
        {
            if (spawnedMainMenu != null)
            {
                spawnedMainMenu.gameObject.SetActive(true);
            }
            else
            {
                spawnedMainMenu = Instantiate(mainMenuPanel);
                spawnedMainMenu.Setup(OnLocalMulti, OnVsAi);
            }
        }

        /// <summary>
        /// Closes the main menu panel.
        /// </summary>
        private void CloseMainMenu()
        {
            if (spawnedMainMenu != null)
            {
                spawnedMainMenu.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Sets up the game for local multiplayer mode, assigning players and starting the game.
        /// </summary>
        private void OnLocalMulti()
        {
            undoButton.gameObject.SetActive(false);
            hintButton.gameObject.SetActive(false);
            _playerAssigner.AssignLocalPlayers();
            CloseMainMenu();
            _gamelogic.ResetGame();
        }

        /// <summary>
        /// Sets up the game for player vs AI mode, assigning players and starting the game.
        /// </summary>
        private void OnVsAi()
        {
            undoButton.gameObject.SetActive(true);
            hintButton.gameObject.SetActive(true);
            _playerAssigner.AssingLocalVsAi();
            CloseMainMenu();
            _gamelogic.ResetGame();
        }

        /// <summary>
        /// Initiates an undo operation to revert the last two moves in the game.
        /// </summary>
        private void OnUndoButton()
        {
            _gamelogic.UndoLastTwoMoves();
        }

        /// <summary>
        /// Activates the hint feature on the game board to assist the current player.
        /// </summary>
        private void OnHintButton()
        {
            boardView.ShowHint();
        }
    }
}
