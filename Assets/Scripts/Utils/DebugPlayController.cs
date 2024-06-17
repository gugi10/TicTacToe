using DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayController : MonoBehaviour
{
    private int currentPlayerId;
    [Inject] private IGameLogic gameLogic;


    private void Awake()
    {
        currentPlayerId = 1;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameLogic.MakeMove(new GameBoardPosition(0, 0), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            gameLogic.MakeMove(new GameBoardPosition(0, 1), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameLogic.MakeMove(new GameBoardPosition(0, 2), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameLogic.MakeMove(new GameBoardPosition(1, 0), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameLogic.MakeMove(new GameBoardPosition(1, 1), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameLogic.MakeMove(new GameBoardPosition(1, 2), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            gameLogic.MakeMove(new GameBoardPosition(2, 0), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            gameLogic.MakeMove(new GameBoardPosition(2, 1), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            gameLogic.MakeMove(new GameBoardPosition(2, 2), currentPlayerId);
            currentPlayerId = currentPlayerId == 2 ? 1 : 2;

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            gameLogic.UndoLastTwoMoves();

        }
    }
}
