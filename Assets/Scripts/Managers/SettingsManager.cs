using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private const string BGM_ENABLED_KEY = "BGMEnabled";
    private const string SFX_ENABLED_KEY = "SFXEnabled";

    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance;

    private bool _bgmEnabled = true;
    private bool _sfxEnabled = true;

    public bool BGMEnabled
    {
        get => _bgmEnabled;
        set
        {
            _bgmEnabled = value;
            PlayerPrefs.SetInt(BGM_ENABLED_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool SFXEnabled
    {
        get => _sfxEnabled;
        set
        {
            _sfxEnabled = value;
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        LoadSettings();
    }

    private void LoadSettings()
    {
        _bgmEnabled = PlayerPrefs.GetInt(BGM_ENABLED_KEY, 1) == 1;
        _sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
    }

    public void ResetToDefaults()
    {
        BGMEnabled = true;
        SFXEnabled = true;
    }
}