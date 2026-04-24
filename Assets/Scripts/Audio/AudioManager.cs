using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _bgmClip;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _placementClip;
    [SerializeField] private AudioClip _strikeClip;
    [SerializeField] private AudioClip _popupClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadAudioClips();
        SetupAudioSources();
    }

    private void LoadAudioClips()
    {
        if (_bgmClip == null) _bgmClip = Resources.Load<AudioClip>("Audio/music");
        if (_clickClip == null) _clickClip = Resources.Load<AudioClip>("Audio/click1");
        if (_placementClip == null) _placementClip = Resources.Load<AudioClip>("Audio/click2");
        if (_strikeClip == null) _strikeClip = Resources.Load<AudioClip>("Audio/woosh");
        if (_popupClip == null) _popupClip = Resources.Load<AudioClip>("Audio/pop");
    }

    private void SetupAudioSources()
    {
        if (_bgmSource == null)
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            _bgmSource.volume = 0.5f;
        }

        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
            _sfxSource.volume = 0.7f;
        }
    }

    private void Start()
    {
        if (SettingsManager.Instance != null)
        {
            UpdateBGMState();
            UpdateSFXState();
        }
    }

    public void PlayBGM()
    {
        if (_bgmSource != null && _bgmClip != null && SettingsManager.Instance?.BGMEnabled == true)
        {
            _bgmSource.clip = _bgmClip;
            _bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (_bgmSource != null)
        {
            _bgmSource.Stop();
        }
    }

    public void PlayClick()
    {
        if (_sfxSource != null && _clickClip != null && SettingsManager.Instance?.SFXEnabled == true)
        {
            _sfxSource.PlayOneShot(_clickClip);
        }
    }

    public void PlayPlacement()
    {
        if (_sfxSource != null && _placementClip != null && SettingsManager.Instance?.SFXEnabled == true)
        {
            _sfxSource.PlayOneShot(_placementClip);
        }
    }

    public void PlayStrike()
    {
        if (_sfxSource != null && _strikeClip != null && SettingsManager.Instance?.SFXEnabled == true)
        {
            _sfxSource.PlayOneShot(_strikeClip);
        }
    }

    public void PlayPopup()
    {
        if (_sfxSource != null && _popupClip != null && SettingsManager.Instance?.SFXEnabled == true)
        {
            _sfxSource.PlayOneShot(_popupClip);
        }
    }

    public void UpdateBGMState()
    {
        if (SettingsManager.Instance?.BGMEnabled == true)
        {
            if (!_bgmSource.isPlaying && _bgmClip != null)
            {
                PlayBGM();
            }
        }
        else
        {
            StopBGM();
        }
    }

    public void UpdateSFXState()
    {
        // SFX is controlled per-call, so no action needed here
    }

    public void SetBGMVolume(float volume)
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (_sfxSource != null)
        {
            _sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}