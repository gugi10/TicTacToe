using UnityEngine;
using System;

/// <summary>
/// Represents a human player in the game, implementing the IPlayer interface.
/// Handles player input via mouse clicks.
/// </summary>
public class HumanPlayer : MonoBehaviour, IPlayer, IPlayerOnInputHandle
{
    /// <summary>
    /// Gets the player's symbol.
    /// </summary>
    public PlayerSymbol PlayerSymbol => _playerSymbol;

    /// <summary>
    /// Gets the player's unique identifier.
    /// </summary>
    public int PlayerId => _playerId;

    private PlayerSymbol _playerSymbol;
    private bool _isYourTurn;
    private Camera _camera;
    private bool _isSetup;
    private Action<IPlayer, GameBoardPosition> _onSuccessfulInput;
    private int _playerId;

    /// <summary>
    /// Sets the camera for the player to use for input.
    /// </summary>
    /// <param name="camera">The camera to be set.</param>
    public void SetCamera(Camera camera)
    {
        _camera = camera;
        _isSetup = true;
    }

    /// <summary>
    /// Sets the callback to be invoked upon a successful input.
    /// </summary>
    /// <param name="onSuccessfullInput">The callback action.</param>
    public void SetCallback(Action<IPlayer, GameBoardPosition> onSuccessfullInput)
    {
        _onSuccessfulInput = onSuccessfullInput;
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
        _isYourTurn = false;
    }

    /// <summary>
    /// Passes priority to the player to make a move.
    /// </summary>
    /// <returns>The human player itself.</returns>
    public IPlayer PassPriority()
    {
        _isYourTurn = true;
        return this;
    }

    /// <summary>
    /// Stops the player from making a move and blocks priority.
    /// </summary>
    /// <returns>The human player itself.</returns>
    public IPlayer BlockPriority()
    {
        _isYourTurn = false;
        return this;
    }

    /// <summary>
    /// Handles player input
    /// </summary>
    private void Update()
    {
        if (!_isSetup)
        {
            return;
        }

        if (!_isYourTurn)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var mousePosWorld2D = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);

            var hit = Physics2D.Raycast(mousePosWorld2D, Vector2.zero);
            if (hit.collider)
            {
                var token = hit.collider.gameObject.GetComponent<Token>();
                if (token == null)
                {
                    Debug.Log("Clicked not on token");
                    return;
                }
                _isYourTurn = false;
                var pos = token.TokenClicked();
                _onSuccessfulInput?.Invoke(this, pos);
            }
        }
    }
}
