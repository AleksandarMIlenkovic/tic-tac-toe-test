using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button statsButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Theme Popup")]
    public GameObject themePopup;
    public Button classicButton;
    public Button neonButton;
    public Button monochromeButton;
    public Button themeStartButton;
    public Button themeCancelButton;

    [Header("Stats Popup")]
    public GameObject statsPopup;
    public TMPro.TextMeshProUGUI statsTotalGames;
    public TMPro.TextMeshProUGUI statsP1Wins;
    public TMPro.TextMeshProUGUI statsP2Wins;
    public TMPro.TextMeshProUGUI statsDraws;
    public TMPro.TextMeshProUGUI statsAvgDuration;
    public Button statsCloseButton;

    [Header("Settings Popup")]
    public GameObject settingsPopup;
    public Toggle bgmToggle;
    public Toggle sfxToggle;
    public Button settingsCloseButton;

    [Header("Exit Popup")]
    public GameObject exitPopup;
    public Button exitCancelButton;
    public Button exitConfirmButton;

    [Header("Game HUD")]
    public GameObject gamePanel;
    public TMPro.TextMeshProUGUI timerText;
    public TMPro.TextMeshProUGUI turnText;
    public TMPro.TextMeshProUGUI p1MovesText;
    public TMPro.TextMeshProUGUI p2MovesText;
    public Button settingsInGameButton;
    public Button backButton;

    [Header("Game Board")]
    public RectTransform boardContainer;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI winnerText;
    public TMPro.TextMeshProUGUI resultText;
    public TMPro.TextMeshProUGUI durationText;
    public Button retryButton;
    public Button exitToMenuButton;

    [Header("Button Art")]
    public Sprite buttonIdle;

    [Header("Board Sprites")]
    public Sprite xRed;
    public Sprite xGreen;
    public Sprite xBlue;
    public Sprite oBlack;
    public Sprite oPurple;
    public Sprite oYellow;

    public MenuController MenuCtrl { get; private set; }
    public BoardView Board { get; private set; }
    public GameHUDController HUD { get; private set; }

    private GameObject[] _allScreens;

    private void Awake()
    {
        BootstrapManager.EnsureManagers();

        if (ThemeManager.Instance != null)
            ThemeManager.Instance.SetSprites(xRed, xGreen, xBlue, oBlack, oPurple, oYellow);

        MenuCtrl = gameObject.AddComponent<MenuController>();
        Board = gameObject.AddComponent<BoardView>();
        HUD = gameObject.AddComponent<GameHUDController>();

        _allScreens = new GameObject[] { mainMenuPanel, themePopup, statsPopup, settingsPopup, exitPopup, gamePanel, gameOverPanel };
    }

    private void Start()
    {
        MenuCtrl.Initialize(
            StartGame, QuitGame, () => gamePanel != null && gamePanel.activeSelf, _allScreens,
            mainMenuPanel, playButton, statsButton, settingsButton, exitButton,
            themePopup, classicButton, neonButton, monochromeButton, themeStartButton, themeCancelButton,
            statsPopup, statsTotalGames, statsP1Wins, statsP2Wins, statsDraws, statsAvgDuration, statsCloseButton,
            settingsPopup, bgmToggle, sfxToggle, settingsCloseButton,
            exitPopup, exitCancelButton, exitConfirmButton,
            buttonIdle);

        HUD.Initialize(
            gamePanel,
            timerText, turnText, p1MovesText, p2MovesText,
            settingsInGameButton, backButton,
            gameOverPanel,
            winnerText, resultText, durationText,
            retryButton, exitToMenuButton);

        MenuCtrl.ShowMainMenu();
    }

    private void Update()
    {
        HUD.UpdateTimer();
    }

    private void StartGame()
    {
        if (!Board.IsCreated)
            Board.CreateBoard(gamePanel);

        StartNewGame();
        RegisterGameButtons();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCellPlaced += OnCellPlaced;
            GameManager.Instance.OnPlayerTurnChanged += OnTurnChanged;
            GameManager.Instance.OnGameEnded += OnGameEnded;
        }

        MenuCtrl.ShowScreen(gamePanel);
    }

    private void StartNewGame()
    {
        CancelInvoke(nameof(ShowGameOverScreen));
        BootstrapManager.EnsureManagers();

        int themeIndex = ThemeManager.Instance != null ? ThemeManager.Instance.SelectedThemeIndex : 0;
        GameManager.Instance.StartNewGame(themeIndex);

        Board.ClearBoard();
        Board.RegisterCellClicks(OnCellClick);
        HUD.StartTimer();
        HUD.ClearGameOver();
        HUD.UpdateTurnDisplay(GameManager.Instance.CurrentPlayer);
        HUD.UpdateMovesDisplay();
    }

    private void RegisterGameButtons()
    {
        if (HUD.SettingsInGameButton != null)
        {
            HUD.SettingsInGameButton.onClick.RemoveAllListeners();
            HUD.SettingsInGameButton.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayClick();
                MenuCtrl.ShowSettings();
            });
        }

        if (HUD.BackButton != null)
        {
            HUD.BackButton.onClick.RemoveAllListeners();
            HUD.BackButton.onClick.AddListener(BackToMenu);
        }

        if (HUD.RetryButton != null)
        {
            HUD.RetryButton.onClick.RemoveAllListeners();
            HUD.RetryButton.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayClick();
                StartNewGame();
                MenuCtrl.ShowScreen(gamePanel);
                RegisterGameButtons();
            });
        }

        if (HUD.ExitToMenuButton != null)
        {
            HUD.ExitToMenuButton.onClick.RemoveAllListeners();
            HUD.ExitToMenuButton.onClick.AddListener(BackToMenu);
        }
    }

    private void OnCellClick(int index)
    {
        if (!HUD.GameActive || GameManager.Instance.Board[index] != 0) return;

        bool isP1 = GameManager.Instance.CurrentPlayer == Player.Player1;
        HUD.RecordMove(isP1);

        int themeIndex = ThemeManager.Instance != null ? ThemeManager.Instance.SelectedThemeIndex : 0;
        Board.PlaceMark(index, isP1, themeIndex);

        AudioManager.Instance?.PlayPlacement();
        GameManager.Instance.TryPlaceMove(index);
    }

    private void OnCellPlaced(int cellIndex) { }

    private void OnTurnChanged(Player player)
    {
        HUD.UpdateTurnDisplay(player);
        HUD.UpdateMovesDisplay();
    }

    private void OnGameEnded(GameResult result, int[] winningLine)
    {
        HUD.StopTimer();

        if (winningLine != null)
            AudioManager.Instance?.PlayStrike();
        else
            AudioManager.Instance?.PlayPopup();

        HUD.ShowGameOver(result, GameManager.Instance.GameDuration);
        Invoke(nameof(ShowGameOverScreen), 0.5f);
    }

    private void ShowGameOverScreen()
    {
        MenuCtrl.ShowScreen(gameOverPanel);
    }

    private void BackToMenu()
    {
        CancelInvoke(nameof(ShowGameOverScreen));
        AudioManager.Instance?.PlayClick();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCellPlaced -= OnCellPlaced;
            GameManager.Instance.OnPlayerTurnChanged -= OnTurnChanged;
            GameManager.Instance.OnGameEnded -= OnGameEnded;
        }

        MenuCtrl.ShowMainMenu();
    }

    private void QuitGame()
    {
        AudioManager.Instance?.PlayClick();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCellPlaced -= OnCellPlaced;
            GameManager.Instance.OnPlayerTurnChanged -= OnTurnChanged;
            GameManager.Instance.OnGameEnded -= OnGameEnded;
        }
    }
}
