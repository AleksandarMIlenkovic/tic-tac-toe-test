using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public event Action<int> OnCellPlaced;
    public event Action<Player> OnPlayerTurnChanged;
    public event Action<GameResult, int[]> OnGameEnded;

    private int[] _board;
    private Player _currentPlayer = Player.Player1;
    private bool _isGameActive;
    private int _moveCount;
    private float _gameStartTime;
    private float _lastGameDuration;
    private int _selectedThemeIndex;

    private static readonly int[][] WinLines = new int[][]
    {
        new int[] { 0, 1, 2 },
        new int[] { 3, 4, 5 },
        new int[] { 6, 7, 8 },
        new int[] { 0, 3, 6 },
        new int[] { 1, 4, 7 },
        new int[] { 2, 5, 8 },
        new int[] { 0, 4, 8 },
        new int[] { 2, 4, 6 }
    };

    public int[] Board => _board;
    public Player CurrentPlayer => _currentPlayer;
    public bool IsGameActive => _isGameActive;
    public int MoveCount => _moveCount;
    public int SelectedThemeIndex => _selectedThemeIndex;

    public float GameDuration => _isGameActive ? Time.time - _gameStartTime : _lastGameDuration;

    private void Awake()
    {
        _instance = this;
        _board = new int[9];
    }

    public void StartNewGame(int themeIndex = 0)
    {
        for (int i = 0; i < 9; i++)
            _board[i] = 0;
        
        _currentPlayer = Player.Player1;
        _isGameActive = true;
        _moveCount = 0;
        _gameStartTime = Time.time;
        _lastGameDuration = 0f;
        _selectedThemeIndex = Mathf.Clamp(themeIndex, 0, 2);
    }

    public bool TryPlaceMove(int cellIndex)
    {
        if (!_isGameActive || cellIndex < 0 || cellIndex >= 9 || _board[cellIndex] != 0)
            return false;

        _board[cellIndex] = (int)_currentPlayer;
        _moveCount++;
        OnCellPlaced?.Invoke(cellIndex);

        if (CheckWin(out int[] winningLine))
        {
            EndGame(_currentPlayer == Player.Player1 ? GameResult.Player1Win : GameResult.Player2Win, winningLine);
            return true;
        }

        if (_moveCount >= 9)
        {
            EndGame(GameResult.Draw, null);
            return true;
        }

        _currentPlayer = _currentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
        OnPlayerTurnChanged?.Invoke(_currentPlayer);
        return true;
    }

    private bool CheckWin(out int[] winningLine)
    {
        winningLine = null;

        foreach (var line in WinLines)
        {
            if (_board[line[0]] != 0 &&
                _board[line[0]] == _board[line[1]] &&
                _board[line[0]] == _board[line[2]])
            {
                winningLine = line;
                return true;
            }
        }

        return false;
    }

    private void EndGame(GameResult result, int[] winningLine)
    {
        _isGameActive = false;
        float duration = Time.time - _gameStartTime;
        _lastGameDuration = duration;

        if (StatsManager.Instance != null)
            StatsManager.Instance.RecordGame(result, duration);

        OnGameEnded?.Invoke(result, winningLine);
    }

    public void EndGameWithoutRecording()
    {
        _isGameActive = false;
    }
}

public enum Player { Player1 = 1, Player2 = 2 }

public static class GameResultExtensions
{
    public static string ToDisplayString(this GameResult result)
    {
        return result switch
        {
            GameResult.Draw => "Draw",
            GameResult.Player1Win => "Player 1 Wins",
            GameResult.Player2Win => "Player 2 Wins",
            _ => ""
        };
    }
}

public enum GameResult { Draw, Player1Win, Player2Win }