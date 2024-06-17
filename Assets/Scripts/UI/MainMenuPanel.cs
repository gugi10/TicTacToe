using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private Button localMultiPlayerStart;
        [SerializeField] private Button aiGameStart;

        private Action _onLocalMultiplayer;
        private Action _onVsAi;

        private void Awake()
        {
            localMultiPlayerStart.onClick.AddListener(() => HandleLocalMulti());
            aiGameStart.onClick.AddListener(() => HandleVsAi());
        }

        public void Setup(Action onLocalMultiPlayer, Action onVsAi)
        {
            _onLocalMultiplayer = onLocalMultiPlayer;
            _onVsAi = onVsAi;
        }

        private void HandleLocalMulti()
        {
            _onLocalMultiplayer?.Invoke();
        }

        private void HandleVsAi()
        {
            _onVsAi?.Invoke();
        }
    }
}