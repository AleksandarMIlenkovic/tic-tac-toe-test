using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TextMeshProUGUI statsTotalGames;
    public TextMeshProUGUI statsP1Wins;
    public TextMeshProUGUI statsP2Wins;
    public TextMeshProUGUI statsDraws;
    public TextMeshProUGUI statsAvgDuration;
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
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI p1MovesText;
    public TextMeshProUGUI p2MovesText;
    public Button settingsInGameButton;
    public Button backButton;

    [Header("Game Board")]
    public RectTransform boardContainer;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI durationText;
    public Button retryButton;
    public Button exitToMenuButton;

    [Header("Board Sprites")]
    public Sprite xRed;
    public Sprite xGreen;
    public Sprite xBlue;
    public Sprite oBlack;
    public Sprite oPurple;
    public Sprite oYellow;

    [Header("Board HUD Colors")]
    public Color xColor = new Color(0.86f, 0.2f, 0.2f);
    public Color oColor = new Color(0.2f, 0.39f, 0.86f);

    [Header("Button Art")]
    public Sprite buttonIdle;
    public Sprite buttonOver;

    private Button[] _cells = new Button[9];
    private Image[] _cellImages = new Image[9];
    private GameObject[] _cellGameObjects = new GameObject[9];

    private int _selectedTheme;
    private int _player1Moves;
    private int _player2Moves;
    private bool _gameActive;
    private float _gameStartTime;
    private bool _boardCreated;

    private GameObject[] _allScreens;

    private void Awake()
    {
        BootstrapManager.EnsureManagers();
        _allScreens = new GameObject[] { mainMenuPanel, themePopup, statsPopup, settingsPopup, exitPopup, gamePanel, gameOverPanel };
    }

    private void ShowScreen(GameObject screen)
    {
        foreach (var s in _allScreens)
            if (s != null) s.SetActive(false);

        if (screen != null)
        {
            screen.SetActive(true);
            screen.transform.SetAsLastSibling();
        }
    }

    private void Start()
    {
        ApplyButtonArt();
        _allScreens = new GameObject[] { mainMenuPanel, themePopup, statsPopup, settingsPopup, exitPopup, gamePanel, gameOverPanel };
        ShowMainMenu();
    }

    private void Update()
    {
        if (!_gameActive || timerText == null) return;

        float dur = Time.time - _gameStartTime;
        int m = Mathf.FloorToInt(dur / 60f);
        int s = Mathf.FloorToInt((int)dur % 60);
        timerText.text = $"{m:00}:{s:00}";
    }

    #region BOARD CREATION

    private void CreateBoard()
    {
        if (gamePanel == null) return;

        if (boardContainer == null)
        {
            var go = new GameObject("BoardContainer");
            go.transform.SetParent(gamePanel.transform);
            boardContainer = go.GetComponent<RectTransform>();
        }

        boardContainer.anchorMin = new Vector2(0.5f, 0.55f);
        boardContainer.anchorMax = new Vector2(0.5f, 0.55f);
        boardContainer.pivot = new Vector2(0.5f, 0.5f);
        boardContainer.anchoredPosition = Vector2.zero;
        boardContainer.sizeDelta = new Vector2(330, 330);

        var grid = boardContainer.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = boardContainer.gameObject.AddComponent<GridLayoutGroup>();

        grid.cellSize = new Vector2(100, 100);
        grid.spacing = new Vector2(10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.childAlignment = TextAnchor.MiddleCenter;


        for (int i = 0; i < 9; i++)
        {
            var cellGO = new GameObject($"Cell{i}", typeof(RectTransform));
            cellGO.transform.SetParent(boardContainer);
            cellGO.transform.localScale = Vector3.one;

            var rect = cellGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var bgImage = cellGO.AddComponent<Image>();
            bgImage.color = new Color(1, 1, 1, 0.2f);

            var btn = cellGO.AddComponent<Button>();
            btn.targetGraphic = bgImage;
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1, 1, 1, 1f);
            cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            cb.disabledColor = Color.white;
            btn.colors = cb;
            btn.transition = Selectable.Transition.ColorTint;

            var markGO = new GameObject("CellMark", typeof(RectTransform));
            markGO.transform.SetParent(cellGO.transform);
            markGO.transform.localScale = Vector3.one;

            var markRect = markGO.GetComponent<RectTransform>();
            markRect.anchorMin = new Vector2(0.15f, 0.15f);
            markRect.anchorMax = new Vector2(0.85f, 0.85f);
            markRect.offsetMin = Vector2.zero;
            markRect.offsetMax = Vector2.zero;

            var markImage = markGO.AddComponent<Image>();
            markImage.color = Color.clear;
            markImage.preserveAspect = true;
            markImage.raycastTarget = false;

            _cells[i] = btn;
            _cellImages[i] = markImage;
            _cellGameObjects[i] = cellGO;
        }

        Debug.Log("[GameController] Board created");
        _boardCreated = true;
    }

    #endregion

    #region MENU STATE

    private void ShowMainMenu()
    {
        _selectedTheme = ThemeManager.Instance != null ? ThemeManager.Instance.SelectedThemeIndex : 0;
        UpdateThemeVisuals();
        RegisterMenuButtons();
        ShowScreen(mainMenuPanel);
    }

    private void RegisterMenuButtons()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(OnPlayClick);

        statsButton.onClick.RemoveAllListeners();
        statsButton.onClick.AddListener(OnStatsClick);

        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(OnSettingsClick);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitClick);

        classicButton.onClick.RemoveAllListeners();
        classicButton.onClick.AddListener(() => SelectTheme(0));

        neonButton.onClick.RemoveAllListeners();
        neonButton.onClick.AddListener(() => SelectTheme(1));

        monochromeButton.onClick.RemoveAllListeners();
        monochromeButton.onClick.AddListener(() => SelectTheme(2));

        themeStartButton.onClick.RemoveAllListeners();
        themeStartButton.onClick.AddListener(StartGame);

        themeCancelButton.onClick.RemoveAllListeners();
        themeCancelButton.onClick.AddListener(() => ShowScreen(mainMenuPanel));

        statsCloseButton.onClick.RemoveAllListeners();
        statsCloseButton.onClick.AddListener(() => ShowScreen(mainMenuPanel));

        bgmToggle.onValueChanged.RemoveAllListeners();
        bgmToggle.onValueChanged.AddListener(val =>
        {
            if (SettingsManager.Instance != null) SettingsManager.Instance.BGMEnabled = val;
            AudioManager.Instance?.UpdateBGMState();
        });

        sfxToggle.onValueChanged.RemoveAllListeners();
        sfxToggle.onValueChanged.AddListener(val =>
        {
            if (SettingsManager.Instance != null) SettingsManager.Instance.SFXEnabled = val;
            AudioManager.Instance?.PlayClick();
        });

        settingsCloseButton.onClick.RemoveAllListeners();
        settingsCloseButton.onClick.AddListener(() =>
        {
            if (gamePanel.activeSelf) ShowScreen(gamePanel);
            else ShowScreen(mainMenuPanel);
        });

        exitCancelButton.onClick.RemoveAllListeners();
        exitCancelButton.onClick.AddListener(() => ShowScreen(mainMenuPanel));

        exitConfirmButton.onClick.RemoveAllListeners();
        exitConfirmButton.onClick.AddListener(QuitGame);
    }

    private void OnPlayClick()
    {
        AudioManager.Instance?.PlayClick();
        ShowScreen(themePopup);
    }

    private void OnStatsClick()
    {
        AudioManager.Instance?.PlayClick();
        RefreshStats();
        ShowScreen(statsPopup);
    }

    private void OnSettingsClick()
    {
        AudioManager.Instance?.PlayClick();
        bgmToggle.isOn = SettingsManager.Instance?.BGMEnabled ?? true;
        sfxToggle.isOn = SettingsManager.Instance?.SFXEnabled ?? true;
        ShowScreen(settingsPopup);
    }

    private void OnExitClick()
    {
        AudioManager.Instance?.PlayClick();
        ShowScreen(exitPopup);
    }

    private void SelectTheme(int index)
    {
        AudioManager.Instance?.PlayClick();
        _selectedTheme = Mathf.Clamp(index, 0, 2);
        UpdateThemeVisuals();
    }

    private void UpdateThemeVisuals()
    {
        SetThemeSelected(classicButton, _selectedTheme == 0);
        SetThemeSelected(neonButton, _selectedTheme == 1);
        SetThemeSelected(monochromeButton, _selectedTheme == 2);
    }

    private void SetThemeSelected(Button btn, bool selected)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        img.color = selected ? Color.green : Color.white;
    }

    private void RefreshStats()
    {
        var stats = StatsManager.Instance;
        if (stats == null) return;

        statsTotalGames.text = stats.TotalGames.ToString();
        statsP1Wins.text = stats.Player1Wins.ToString();
        statsP2Wins.text = stats.Player2Wins.ToString();
        statsDraws.text = stats.Draws.ToString();

        int m = Mathf.FloorToInt(stats.AverageDuration / 60f);
        int s = Mathf.FloorToInt((int)stats.AverageDuration % 60);
        statsAvgDuration.text = $"{m:00}:{s:00}";
    }

    #endregion

    #region GAME STATE

    private void StartGame()
    {
        AudioManager.Instance?.PlayClick();

        if (ThemeManager.Instance != null)
            ThemeManager.Instance.SelectedThemeIndex = _selectedTheme;

        if (!_boardCreated)
            CreateBoard();

        StartNewGame();

        RegisterGameButtons();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCellPlaced += OnCellPlaced;
            GameManager.Instance.OnPlayerTurnChanged += OnTurnChanged;
            GameManager.Instance.OnGameEnded += OnGameEnded;
        }

        ShowScreen(gamePanel);
    }

    private void StartNewGame()
    {
        CancelInvoke(nameof(ShowGameOver));

        BootstrapManager.EnsureManagers();

        GameManager.Instance.StartNewGame(_selectedTheme);

        _player1Moves = 0;
        _player2Moves = 0;
        _gameActive = true;
        _gameStartTime = Time.time;

        for (int i = 0; i < 9; i++)
        {
            if (_cellImages[i] != null) _cellImages[i].sprite = null;
            if (_cellImages[i] != null) _cellImages[i].color = Color.clear;
            if (_cells[i] != null) _cells[i].interactable = true;
        }

        if (winnerText != null)
            winnerText.text = "";

        gameOverPanel.SetActive(false);

        UpdateHUD();
    }

    private void RegisterGameButtons()
    {
        for (int i = 0; i < 9; i++)
        {
            int idx = i;
            if (_cells[i] == null) continue;
            _cells[i].onClick.RemoveAllListeners();
            _cells[i].onClick.AddListener(() => OnCellClick(idx));
        }

        if (settingsInGameButton != null)
        {
        settingsInGameButton.onClick.RemoveAllListeners();
        settingsInGameButton.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlayClick();
            bgmToggle.isOn = SettingsManager.Instance?.BGMEnabled ?? true;
            sfxToggle.isOn = SettingsManager.Instance?.SFXEnabled ?? true;
            ShowScreen(settingsPopup);
        });
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(BackToMenu);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayClick();
                StartNewGame();
                ShowScreen(gamePanel);
                RegisterGameButtons();
            });
        }

        if (exitToMenuButton != null)
        {
            exitToMenuButton.onClick.RemoveAllListeners();
            exitToMenuButton.onClick.AddListener(BackToMenu);
        }
    }

    private void OnCellClick(int index)
    {
        if (!_gameActive || GameManager.Instance.Board[index] != 0) return;

        bool isP1 = GameManager.Instance.CurrentPlayer == Player.Player1;
        if (isP1) _player1Moves++; else _player2Moves++;

        Sprite[] xSprites = { xRed, xGreen, xBlue };
        Sprite[] oSprites = { oBlack, oPurple, oYellow };

        Sprite markSprite = isP1 ? xSprites[_selectedTheme] : oSprites[_selectedTheme];

        if (markSprite == null)
            Debug.LogError($"[GameController] Sprite is null! isP1: {isP1}, theme: {_selectedTheme}. Assign sprites in Inspector.");

        _cellImages[index].sprite = markSprite;
        _cellImages[index].color = markSprite != null ? Color.white : Color.red;
        _cells[index].interactable = false;

        AudioManager.Instance?.PlayPlacement();

        GameManager.Instance.TryPlaceMove(index);
    }

    private void OnCellPlaced(int cellIndex) { }

    private void OnTurnChanged(Player player)
    {
        UpdateHUD();
    }

    private void OnGameEnded(GameResult result, int[] winningLine)
    {
        _gameActive = false;

        if (winningLine != null)
            AudioManager.Instance?.PlayStrike();
        else
            AudioManager.Instance?.PlayPopup();

        if (winnerText != null)
        {
            winnerText.text = result.ToDisplayString();
            winnerText.color = result == GameResult.Draw ? Color.gray :
                (result == GameResult.Player1Win ? xColor : oColor);
        }

        resultText.text = "Game Over";

        float dur = GameManager.Instance.GameDuration;
        int m = Mathf.FloorToInt(dur / 60f);
        int s = Mathf.FloorToInt((int)dur % 60);
        durationText.text = $"Duration: {m:00}:{s:00}";

        Invoke(nameof(ShowGameOver), 0.5f);
    }

    private void ShowGameOver()
    {
        ShowScreen(gameOverPanel);
    }

    private void UpdateHUD()
    {
        var p = GameManager.Instance.CurrentPlayer;
        if (turnText != null)
        {
            turnText.text = p == Player.Player1 ? "Player 1's Turn" : "Player 2's Turn";
            turnText.color = p == Player.Player1 ? xColor : oColor;
        }
        if (p1MovesText != null) p1MovesText.text = $"Moves: {_player1Moves}";
        if (p2MovesText != null) p2MovesText.text = $"Moves: {_player2Moves}";
    }

    private void BackToMenu()
    {
        CancelInvoke(nameof(ShowGameOver));

        AudioManager.Instance?.PlayClick();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCellPlaced -= OnCellPlaced;
            GameManager.Instance.OnPlayerTurnChanged -= OnTurnChanged;
            GameManager.Instance.OnGameEnded -= OnGameEnded;
        }

        ShowScreen(mainMenuPanel);
    }

    #endregion

    #region BUTTON ART

    private void ApplyButtonArt()
    {
        if (buttonIdle == null) return;

        Button[] allButtons = new Button[]
        {
            playButton, statsButton, settingsButton, exitButton,
            classicButton, neonButton, monochromeButton, themeStartButton, themeCancelButton,
            statsCloseButton, settingsCloseButton, exitCancelButton, exitConfirmButton,
            settingsInGameButton, backButton, retryButton, exitToMenuButton
        };

        foreach (var btn in allButtons)
        {
            if (btn == null) continue;
            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = buttonIdle;
                img.type = Image.Type.Sliced;
            }

            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            btn.colors = cb;
        }
    }

    #endregion

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
