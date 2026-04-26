using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private GameObject _mainMenuPanel;
    private Button _playButton;
    private Button _statsButton;
    private Button _settingsButton;
    private Button _exitButton;

    private GameObject _themePopup;
    private Button _classicButton;
    private Button _neonButton;
    private Button _monochromeButton;
    private Button _themeStartButton;
    private Button _themeCancelButton;

    private GameObject _statsPopup;
    private TextMeshProUGUI _statsTotalGames;
    private TextMeshProUGUI _statsP1Wins;
    private TextMeshProUGUI _statsP2Wins;
    private TextMeshProUGUI _statsDraws;
    private TextMeshProUGUI _statsAvgDuration;
    private Button _statsCloseButton;

    private GameObject _settingsPopup;
    private Toggle _bgmToggle;
    private Toggle _sfxToggle;
    private Button _settingsCloseButton;

    private GameObject _exitPopup;
    private Button _exitCancelButton;
    private Button _exitConfirmButton;

    private Sprite _buttonIdle;

    private int _selectedTheme;
    private GameObject[] _allScreens;
    private System.Action _onPlayStart;
    private System.Action _onQuit;
    private System.Func<bool> _isInGame;

    public Toggle BgmToggle => _bgmToggle;
    public Toggle SfxToggle => _sfxToggle;
    public GameObject SettingsPopup => _settingsPopup;

    public void Initialize(
        System.Action onPlayStart,
        System.Action onQuit,
        System.Func<bool> isInGame,
        GameObject[] allScreens,
        GameObject mainMenuPanel, Button playButton, Button statsButton, Button settingsButton, Button exitButton,
        GameObject themePopup, Button classicButton, Button neonButton, Button monochromeButton, Button themeStartButton, Button themeCancelButton,
        GameObject statsPopup, TextMeshProUGUI statsTotalGames, TextMeshProUGUI statsP1Wins, TextMeshProUGUI statsP2Wins, TextMeshProUGUI statsDraws, TextMeshProUGUI statsAvgDuration, Button statsCloseButton,
        GameObject settingsPopup, Toggle bgmToggle, Toggle sfxToggle, Button settingsCloseButton,
        GameObject exitPopup, Button exitCancelButton, Button exitConfirmButton,
        Sprite buttonIdle)
    {
        _onPlayStart = onPlayStart;
        _onQuit = onQuit;
        _isInGame = isInGame;
        _allScreens = allScreens;

        _mainMenuPanel = mainMenuPanel;
        _playButton = playButton;
        _statsButton = statsButton;
        _settingsButton = settingsButton;
        _exitButton = exitButton;

        _themePopup = themePopup;
        _classicButton = classicButton;
        _neonButton = neonButton;
        _monochromeButton = monochromeButton;
        _themeStartButton = themeStartButton;
        _themeCancelButton = themeCancelButton;

        _statsPopup = statsPopup;
        _statsTotalGames = statsTotalGames;
        _statsP1Wins = statsP1Wins;
        _statsP2Wins = statsP2Wins;
        _statsDraws = statsDraws;
        _statsAvgDuration = statsAvgDuration;
        _statsCloseButton = statsCloseButton;

        _settingsPopup = settingsPopup;
        _bgmToggle = bgmToggle;
        _sfxToggle = sfxToggle;
        _settingsCloseButton = settingsCloseButton;

        _exitPopup = exitPopup;
        _exitCancelButton = exitCancelButton;
        _exitConfirmButton = exitConfirmButton;

        _buttonIdle = buttonIdle;

        ApplyButtonArt();
        RegisterButtons();
    }

    public void ShowMainMenu()
    {
        _selectedTheme = ThemeManager.Instance != null ? ThemeManager.Instance.SelectedThemeIndex : 0;
        UpdateThemeVisuals();
        ShowScreen(_mainMenuPanel);
    }

    public void ShowSettings()
    {
        _bgmToggle.isOn = SettingsManager.Instance?.BGMEnabled ?? true;
        _sfxToggle.isOn = SettingsManager.Instance?.SFXEnabled ?? true;
        ShowScreen(_settingsPopup);
    }

    private void RegisterButtons()
    {
        _playButton.onClick.RemoveAllListeners();
        _playButton.onClick.AddListener(OnPlayClick);

        _statsButton.onClick.RemoveAllListeners();
        _statsButton.onClick.AddListener(OnStatsClick);

        _settingsButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.AddListener(OnSettingsClick);

        _exitButton.onClick.RemoveAllListeners();
        _exitButton.onClick.AddListener(OnExitClick);

        _classicButton.onClick.RemoveAllListeners();
        _classicButton.onClick.AddListener(() => SelectTheme(0));

        _neonButton.onClick.RemoveAllListeners();
        _neonButton.onClick.AddListener(() => SelectTheme(1));

        _monochromeButton.onClick.RemoveAllListeners();
        _monochromeButton.onClick.AddListener(() => SelectTheme(2));

        _themeStartButton.onClick.RemoveAllListeners();
        _themeStartButton.onClick.AddListener(OnThemeStart);

        _themeCancelButton.onClick.RemoveAllListeners();
        _themeCancelButton.onClick.AddListener(() => ShowScreen(_mainMenuPanel));

        _statsCloseButton.onClick.RemoveAllListeners();
        _statsCloseButton.onClick.AddListener(() => ShowScreen(_mainMenuPanel));

        _bgmToggle.onValueChanged.RemoveAllListeners();
        _bgmToggle.onValueChanged.AddListener(OnBgmToggle);

        _sfxToggle.onValueChanged.RemoveAllListeners();
        _sfxToggle.onValueChanged.AddListener(OnSfxToggle);

        _settingsCloseButton.onClick.RemoveAllListeners();
        _settingsCloseButton.onClick.AddListener(OnSettingsClose);

        _exitCancelButton.onClick.RemoveAllListeners();
        _exitCancelButton.onClick.AddListener(() => ShowScreen(_mainMenuPanel));

        _exitConfirmButton.onClick.RemoveAllListeners();
        _exitConfirmButton.onClick.AddListener(OnQuitConfirm);
    }

    private void OnPlayClick()
    {
        AudioManager.Instance?.PlayClick();
        ShowScreen(_themePopup);
    }

    private void OnStatsClick()
    {
        AudioManager.Instance?.PlayClick();
        RefreshStats();
        ShowScreen(_statsPopup);
    }

    private void OnSettingsClick()
    {
        AudioManager.Instance?.PlayClick();
        ShowSettings();
    }

    private void OnExitClick()
    {
        AudioManager.Instance?.PlayClick();
        ShowScreen(_exitPopup);
    }

    private void OnThemeStart()
    {
        AudioManager.Instance?.PlayClick();
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.SelectedThemeIndex = _selectedTheme;
        _onPlayStart?.Invoke();
    }

    private void OnBgmToggle(bool val)
    {
        if (SettingsManager.Instance != null) SettingsManager.Instance.BGMEnabled = val;
        AudioManager.Instance?.UpdateBGMState();
    }

    private void OnSfxToggle(bool val)
    {
        if (SettingsManager.Instance != null) SettingsManager.Instance.SFXEnabled = val;
        AudioManager.Instance?.PlayClick();
    }

    private void OnSettingsClose()
    {
        ShowScreen(_mainMenuPanel);
    }

    private void OnQuitConfirm()
    {
        AudioManager.Instance?.PlayClick();
        _onQuit?.Invoke();
    }

    private void SelectTheme(int index)
    {
        AudioManager.Instance?.PlayClick();
        _selectedTheme = Mathf.Clamp(index, 0, 2);
        UpdateThemeVisuals();
    }

    private void UpdateThemeVisuals()
    {
        SetThemeSelected(_classicButton, _selectedTheme == 0);
        SetThemeSelected(_neonButton, _selectedTheme == 1);
        SetThemeSelected(_monochromeButton, _selectedTheme == 2);
    }

    private void SetThemeSelected(Button btn, bool selected)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null) img.color = selected ? Color.green : Color.white;
    }

    private void RefreshStats()
    {
        var stats = StatsManager.Instance;
        if (stats == null) return;

        _statsTotalGames.text = stats.TotalGames.ToString();
        _statsP1Wins.text = stats.Player1Wins.ToString();
        _statsP2Wins.text = stats.Player2Wins.ToString();
        _statsDraws.text = stats.Draws.ToString();

        int m = Mathf.FloorToInt(stats.AverageDuration / 60f);
        int s = Mathf.FloorToInt((int)stats.AverageDuration % 60);
        _statsAvgDuration.text = $"{m:00}:{s:00}";
    }

    public void ShowScreen(GameObject screen)
    {
        if (_allScreens == null) return;
        foreach (var s in _allScreens)
            if (s != null) s.SetActive(false);
        if (screen != null)
        {
            screen.SetActive(true);
            screen.transform.SetAsLastSibling();
        }
    }

    private void ApplyButtonArt()
    {
        if (_buttonIdle == null) return;

        Button[] allButtons = new Button[]
        {
            _playButton, _statsButton, _settingsButton, _exitButton,
            _classicButton, _neonButton, _monochromeButton, _themeStartButton, _themeCancelButton,
            _statsCloseButton, _settingsCloseButton, _exitCancelButton, _exitConfirmButton
        };

        foreach (var btn in allButtons)
        {
            if (btn == null) continue;
            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = _buttonIdle;
                img.type = Image.Type.Sliced;
            }
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            btn.colors = cb;
        }
    }
}
