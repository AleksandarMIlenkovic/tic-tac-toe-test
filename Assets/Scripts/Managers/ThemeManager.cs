using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    public Theme[] Themes { get; private set; }
    public int SelectedThemeIndex { get; set; }

    public Theme CurrentTheme => Themes != null && SelectedThemeIndex < Themes.Length
        ? Themes[SelectedThemeIndex]
        : null;

    private bool _initialized;

    public void SetSprites(Sprite xRed, Sprite xGreen, Sprite xBlue, Sprite oBlack, Sprite oPurple, Sprite oYellow)
    {
        Themes = new Theme[]
        {
            new Theme(0, "Classic", xGreen, oYellow, new Color(0.2f, 0.8f, 0.2f), new Color(0.9f, 0.8f, 0.1f)),
            new Theme(1, "Neon", xBlue, oPurple, new Color(0.2f, 0.4f, 1f), new Color(0.6f, 0.2f, 1f)),
            new Theme(2, "Monochrome", xRed, oBlack, new Color(0.86f, 0.2f, 0.2f), new Color(0.3f, 0.3f, 0.3f))
        };

        _initialized = true;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SelectedThemeIndex = PlayerPrefs.GetInt("SelectedTheme", 0);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            PlayerPrefs.SetInt("SelectedTheme", SelectedThemeIndex);
            PlayerPrefs.Save();
        }
    }

    public Sprite GetXSprite(int themeIndex)
    {
        if (!_initialized || Themes == null) return null;
        int i = Mathf.Clamp(themeIndex, 0, Themes.Length - 1);
        return Themes[i].XSprite;
    }

    public Sprite GetOSprite(int themeIndex)
    {
        if (!_initialized || Themes == null) return null;
        int i = Mathf.Clamp(themeIndex, 0, Themes.Length - 1);
        return Themes[i].OSprite;
    }
}

[System.Serializable]
public class Theme
{
    public int Id;
    public string Name;
    public Sprite XSprite;
    public Sprite OSprite;
    public Color XColor;
    public Color OColor;

    public Theme(int id, string name, Sprite xSprite, Sprite oSprite, Color xColor, Color oColor)
    {
        Id = id;
        Name = name;
        XSprite = xSprite;
        OSprite = oSprite;
        XColor = xColor;
        OColor = oColor;
    }
}
