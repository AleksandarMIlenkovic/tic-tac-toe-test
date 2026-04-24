using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private const string TOTAL_GAMES_KEY = "TotalGames";
    private const string PLAYER1_WINS_KEY = "Player1Wins";
    private const string PLAYER2_WINS_KEY = "Player2Wins";
    private const string DRAWS_KEY = "Draws";
    private const string TOTAL_DURATION_KEY = "TotalDuration";

    private static StatsManager _instance;
    public static StatsManager Instance => _instance;

    private int _totalGames;
    private int _player1Wins;
    private int _player2Wins;
    private int _draws;
    private float _totalDuration;

    public int TotalGames => _totalGames;
    public int Player1Wins => _player1Wins;
    public int Player2Wins => _player2Wins;
    public int Draws => _draws;
    public float TotalDuration => _totalDuration;

    public float AverageDuration => _totalGames > 0 ? _totalDuration / _totalGames : 0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        
        LoadStats();
    }

    private void LoadStats()
    {
        _totalGames = PlayerPrefs.GetInt(TOTAL_GAMES_KEY, 0);
        _player1Wins = PlayerPrefs.GetInt(PLAYER1_WINS_KEY, 0);
        _player2Wins = PlayerPrefs.GetInt(PLAYER2_WINS_KEY, 0);
        _draws = PlayerPrefs.GetInt(DRAWS_KEY, 0);
        _totalDuration = PlayerPrefs.GetFloat(TOTAL_DURATION_KEY, 0f);
    }

    public void RecordGame(GameResult result, float gameDuration)
    {
        _totalGames++;
        _totalDuration += gameDuration;

        switch (result)
        {
            case GameResult.Player1Win:
                _player1Wins++;
                break;
            case GameResult.Player2Win:
                _player2Wins++;
                break;
            case GameResult.Draw:
                _draws++;
                break;
        }

        SaveStats();
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt(TOTAL_GAMES_KEY, _totalGames);
        PlayerPrefs.SetInt(PLAYER1_WINS_KEY, _player1Wins);
        PlayerPrefs.SetInt(PLAYER2_WINS_KEY, _player2Wins);
        PlayerPrefs.SetInt(DRAWS_KEY, _draws);
        PlayerPrefs.SetFloat(TOTAL_DURATION_KEY, _totalDuration);
        PlayerPrefs.Save();
    }

    public void ResetStats()
    {
        _totalGames = 0;
        _player1Wins = 0;
        _player2Wins = 0;
        _draws = 0;
        _totalDuration = 0f;
        SaveStats();
    }
}