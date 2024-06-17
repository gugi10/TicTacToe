using Auxiliary.Observables;
using DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the visual representation of the tic-tac-toe board, including the placement and updating of tokens and hint tokens.
/// Observes changes in the game board state and updates the view accordingly.
/// </summary>
public class BoardView : MonoBehaviour
{
    /// <summary>
    /// Gets the array of positions on the board.
    /// </summary>
    public Vector3[,] PositionsArray { get => _positionsArray; }

    /// <summary>
    /// The starting position for the top-left corner of the board.
    /// </summary>
    private readonly Vector3 topLeftCornerStartingPosition = new Vector3(-1.88f, 2.85f, 0);

    /// <summary>
    /// The step size for positioning the tokens.
    /// </summary>
    private const float STEP = 1.88f;

    /// <summary>
    /// Prefab for the tokens used on the board.
    /// </summary>
    [SerializeField] private Token tokenPrefab;

    /// <summary>
    /// Prefab for the hint token.
    /// </summary>
    [SerializeField] private GameObject hintTokenPrefab;

    [Inject] private IGameLogic _gameLogic;
    [Inject] private IConfigService _configService;

    private Vector3[,] _positionsArray;
    private TokenConfig _tokenConfig;
    private Observer<int[,]> gameBoardObserver = new();
    private Token[,] spawnedBoardElements;
    private GameObject pooledHintToken;

    private Coroutine hintCoroutine;

    /// <summary>
    /// Initializes the board view, setting up the positions and tokens, and subscribing to game board changes.
    /// </summary>
    private void Awake()
    {
        _tokenConfig = _configService.GetConfig<TokenConfig>();
        var gameBoard = _gameLogic?.GameBoard.Value;
        if (gameBoard == null)
        {
            Debug.LogError($"{_gameLogic.GetType().Name} board is null cannot setup board view");
            return;
        }
        int numberOfRows = gameBoard.GetLength(0);
        int numberOfColumns = gameBoard.GetLength(1);
        spawnedBoardElements = new Token[numberOfRows, numberOfColumns];
        _positionsArray = new Vector3[numberOfRows, numberOfColumns];
        for (int i = 0; i < numberOfRows; i++)
        {
            float ySpawnPosition = topLeftCornerStartingPosition.y - STEP * (i);
            for (int j = 0; j < numberOfColumns; j++)
            {
                var spawnedToken = Instantiate(tokenPrefab, transform);
                var newPos = new Vector3(topLeftCornerStartingPosition.x + STEP * j,
                    ySpawnPosition,
                    topLeftCornerStartingPosition.z);
                _positionsArray[i, j] = newPos;
                spawnedToken.transform.localPosition = newPos;
                spawnedToken.gameObject.name = $"Token_{i}{j}";
                spawnedToken.Setup(new GameBoardPosition(i, j));
                spawnedToken.Hide();
                spawnedBoardElements[i, j] = spawnedToken;
            }
        }

        gameBoardObserver.AddOnNextListener(val => UpdateView(val)).Observe(_gameLogic.GameBoard, false);
    }

    /// <summary>
    /// Shows a hint on the board by highlighting a suggested move position.
    /// </summary>
    public void ShowHint()
    {
        if (pooledHintToken == null)
        {
            pooledHintToken = Instantiate(hintTokenPrefab);
            pooledHintToken.SetActive(false);
        }

        var point = _gameLogic.GetHintMove();
        if (_gameLogic.CurrentGameState.Value == GameState.Draw || point.columnIndex == -1)
        {
            Debug.Log($"Cannot get hint if game is ended by a draw, or board is full");
            return;
        }
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
        }
        hintCoroutine = StartCoroutine(KillHintAfterTime(point));
    }

    /// <summary>
    /// Coroutine to hide the hint token after a certain period of time.
    /// </summary>
    /// <param name="indexPos">The position of the hint token on the board.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator KillHintAfterTime(GameBoardPosition indexPos)
    {
        pooledHintToken.transform.position = _positionsArray[indexPos.rowIndex, indexPos.columnIndex];
        pooledHintToken.SetActive(true);
        yield return new WaitForSeconds(1f);
        pooledHintToken.SetActive(false);
    }

    /// <summary>
    /// Updates the board view based on the current state of the game board.
    /// </summary>
    /// <param name="gameBoard">The current game board state.</param>
    private void UpdateView(int[,] gameBoard)
    {
        for (int i = 0; i < gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < gameBoard.GetLength(1); j++)
            {
                var spriteToSet = _tokenConfig.GetSpriteByPlayer((PlayerSymbol)gameBoard[i, j]);
                if (spriteToSet == null)
                {
                    spawnedBoardElements[i, j].Hide();
                    continue;
                }
                spawnedBoardElements[i, j].SetSprite(spriteToSet);
            }
        }
    }
}
