using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[TestFixture]
public class TestTicTacToeGameLogic
{

    [SetUp]
    public void SetUp()
    {

    }

    [Test]
    public void TestCrossPlayerLost()
    {
        var _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);
        int movesMade = 0;

        for (int i = 0; i < _gameLogic.GameBoard.Value.GetLength(0); i++)
        {
            for (int j = 0; j < _gameLogic.GameBoard.Value.GetLength(1); j++)
            {
                Debug.Log($"{i}{j}");
                if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 0 && j == 2))
                {
                    _gameLogic.MakeMove(new GameBoardPosition(i, j), 2);
                    movesMade++;
                    if (movesMade == 3)
                    {
                        Assert.AreEqual(GameState.CirclePlayerWon, _gameLogic.CurrentGameState.Value);
                        break;
                    }
                }
            }
        }
    }

    [Test]
    public void TestCrossPlayerWin()
    {
        var _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);
        int movesMade = 0;

        for (int i = 0; i < _gameLogic.GameBoard.Value.GetLength(0); i++)
        {
            for (int j = 0; j < _gameLogic.GameBoard.Value.GetLength(1); j++)
            {
                Debug.Log($"{i}{j}");
                if ((i == 1 && j == 0) || (i == 1 && j == 1) || (i == 1 && j == 2))
                {
                    _gameLogic.MakeMove(new GameBoardPosition(i, j), 1);
                    movesMade++;
                    if (movesMade == 3)
                    {
                        Assert.AreEqual(GameState.CrossPlayerWon, _gameLogic.CurrentGameState.Value);
                        break;
                    }
                }
            }
        }
    }

    [Test]
    public void TestDraw()
    {
        TicTacToeGameLogic _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);

        _gameLogic.MakeMove(new GameBoardPosition(0, 0), 1);
        _gameLogic.MakeMove(new GameBoardPosition(1, 0), 2);
        _gameLogic.MakeMove(new GameBoardPosition(0, 1), 1);
        _gameLogic.MakeMove(new GameBoardPosition(1, 1), 2);
        _gameLogic.MakeMove(new GameBoardPosition(1, 2), 1);
        _gameLogic.MakeMove(new GameBoardPosition(0, 2), 2);
        _gameLogic.MakeMove(new GameBoardPosition(2, 0), 1);
        _gameLogic.MakeMove(new GameBoardPosition(2, 1), 2);
        _gameLogic.MakeMove(new GameBoardPosition(2, 2), 1);
        _gameLogic.MakeMove(new GameBoardPosition(1, 1), 2);
        Assert.AreEqual(GameState.Draw, _gameLogic.CurrentGameState.Value);
    }

    [Test]
    public void TestUndoLastMove()
    {
        var _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);
        var position1 = new GameBoardPosition(0, 0);
        var position2 = new GameBoardPosition(1, 0);

        _gameLogic.MakeMove(position1, 1);
        _gameLogic.MakeMove(position2, 2);
        _gameLogic.UndoLastTwoMoves();

        int[,] expectedBoard = new int[3, 3];
        expectedBoard[0, 0] = 0;

        CollectionAssert.AreEqual(expectedBoard, _gameLogic.GameBoard.Value);
    }

    [Test]
    public void TestResetGame()
    {
        var _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);
        var position = new GameBoardPosition(0, 0);
        _gameLogic.MakeMove(position, 1);
        _gameLogic.ResetGame();

        int[,] expectedBoard = new int[3, 3];
        CollectionAssert.AreEqual(expectedBoard, _gameLogic.GameBoard.Value);
        Assert.AreEqual(GameState.GameStarted, _gameLogic.CurrentGameState.Value);
    }

    [Test]
    public void TestHintPossible()
    {
        var _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);
        int[,] board = new int[3, 3];
        for (int i = 0; i < _gameLogic.GameBoard.Value.GetLength(0); i++)
        {
            for (int j = 0; j < _gameLogic.GameBoard.Value.GetLength(1); j++)
            {
                if (i == 1 && j == 1)
                {
                    //make sure that middle element in board remains empty
                    _gameLogic.MakeMove(new GameBoardPosition(i, j), 0);
                }
                else
                {
                    //simualate alternating moves
                    _gameLogic.MakeMove(new GameBoardPosition(i, j), (i + j) % 2 == 0 ? 1 : 2);
                }
            }
        }
        var point = _gameLogic.GetHintMove();
        Assert.AreEqual(1, point.rowIndex);
        Assert.AreEqual(1, point.columnIndex);

    }

    [Test]
    public void TestHintImpossible()
    {
        var _gameLogic = new TicTacToeGameLogic();
        _gameLogic.CurrentGameState.Set(GameState.GameStarted);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _gameLogic.MakeMove(new GameBoardPosition(i, j), 1);
            }
        }

        var point = _gameLogic.GetHintMove();
        Assert.AreNotEqual(-1, point.columnIndex);
        Assert.AreNotEqual(-1, point.rowIndex);
    }
}