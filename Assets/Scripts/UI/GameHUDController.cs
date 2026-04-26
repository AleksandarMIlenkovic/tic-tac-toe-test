using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDController : MonoBehaviour
{
    private GameObject _gamePanel;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _turnText;
    private TextMeshProUGUI _p1MovesText;
    private TextMeshProUGUI _p2MovesText;
    private Button _settingsInGameButton;
    private Button _backButton;

    private GameObject _gameOverPanel;
    private TextMeshProUGUI _winnerText;
    private TextMeshProUGUI _resultText;
    private TextMeshProUGUI _durationText;
    private Button _retryButton;
    private Button _exitToMenuButton;

    private int _player1Moves;
    private int _player2Moves;
    private bool _gameActive;
    private float _gameStartTime;

    public bool GameActive => _gameActive;
    public Button SettingsInGameButton => _settingsInGameButton;
    public Button BackButton => _backButton;
    public Button RetryButton => _retryButton;
    public Button ExitToMenuButton => _exitToMenuButton;

    public void Initialize(
        GameObject gamePanel,
        TextMeshProUGUI timerText, TextMeshProUGUI turnText, TextMeshProUGUI p1MovesText, TextMeshProUGUI p2MovesText,
        Button settingsInGameButton, Button backButton,
        GameObject gameOverPanel,
        TextMeshProUGUI winnerText, TextMeshProUGUI resultText, TextMeshProUGUI durationText,
        Button retryButton, Button exitToMenuButton)
    {
        _gamePanel = gamePanel;
        _timerText = timerText;
        _turnText = turnText;
        _p1MovesText = p1MovesText;
        _p2MovesText = p2MovesText;
        _settingsInGameButton = settingsInGameButton;
        _backButton = backButton;

        _gameOverPanel = gameOverPanel;
        _winnerText = winnerText;
        _resultText = resultText;
        _durationText = durationText;
        _retryButton = retryButton;
        _exitToMenuButton = exitToMenuButton;
    }

    public void StartTimer()
    {
        _gameActive = true;
        _gameStartTime = Time.time;
        _player1Moves = 0;
        _player2Moves = 0;
    }

    public void StopTimer()
    {
        _gameActive = false;
    }

    public void RecordMove(bool isPlayer1)
    {
        if (isPlayer1) _player1Moves++; else _player2Moves++;
    }

    public void UpdateTimer()
    {
        if (!_gameActive || _timerText == null) return;

        float dur = Time.time - _gameStartTime;
        int m = Mathf.FloorToInt(dur / 60f);
        int s = Mathf.FloorToInt((int)dur % 60);
        _timerText.text = $"{m:00}:{s:00}";
    }

    public void UpdateTurnDisplay(Player currentPlayer)
    {
        if (_turnText == null) return;

        var theme = ThemeManager.Instance?.CurrentTheme;
        Color xColor = theme != null ? theme.XColor : Color.red;
        Color oColor = theme != null ? theme.OColor : Color.blue;

        _turnText.text = currentPlayer == Player.Player1 ? "Player 1's Turn" : "Player 2's Turn";
        _turnText.color = currentPlayer == Player.Player1 ? xColor : oColor;
    }

    public void UpdateMovesDisplay()
    {
        if (_p1MovesText != null) _p1MovesText.text = $"Moves: {_player1Moves}";
        if (_p2MovesText != null) _p2MovesText.text = $"Moves: {_player2Moves}";
    }

    public void ShowGameOver(GameResult result, float gameDuration)
    {
        if (_winnerText != null)
        {
            _winnerText.text = result.ToDisplayString();
            var theme = ThemeManager.Instance?.CurrentTheme;
            Color xColor = theme != null ? theme.XColor : Color.red;
            Color oColor = theme != null ? theme.OColor : Color.blue;
            _winnerText.color = result == GameResult.Draw ? Color.gray
                : (result == GameResult.Player1Win ? xColor : oColor);
        }

        if (_resultText != null) _resultText.text = "Game Over";

        int m = Mathf.FloorToInt(gameDuration / 60f);
        int s = Mathf.FloorToInt((int)gameDuration % 60);
        if (_durationText != null) _durationText.text = $"Duration: {m:00}:{s:00}";
    }

    public void ClearGameOver()
    {
        if (_winnerText != null) _winnerText.text = "";
        if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
    }
}
