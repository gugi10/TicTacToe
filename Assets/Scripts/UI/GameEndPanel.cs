using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UI
{
    public class GameEndPanel : MonoBehaviour
    {
        private const string PLAYER_WINS = "Player {0} wins";
        private const string DRAW_TEXT = "Draw";

        [SerializeField] private Button restartButton;
        [SerializeField] private TextMeshProUGUI endGameText;

        private Action<GameEndPanel> _onRestart;


        private void OnEnable()
        {
            restartButton.onClick.AddListener(HandleOnResetButton);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(HandleOnResetButton);
        }

        public void Setup(GameState gameState, int winningPlayerId, Action<GameEndPanel> onRestart)
        {
            _onRestart = onRestart;
            endGameText.text = gameState switch
            {
                GameState.Draw => DRAW_TEXT,
                GameState.CirclePlayerWon => string.Format(PLAYER_WINS, winningPlayerId),
                GameState.CrossPlayerWon => string.Format(PLAYER_WINS, winningPlayerId),
                _ => $"Something went wrong, unrecognizble gamestate for this screen {gameState}"
            };
        }

        private void HandleOnResetButton()
        {
            Debug.Log($"On reset click");
            _onRestart?.Invoke(this);
        }
    }
}