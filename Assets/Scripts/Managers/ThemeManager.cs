using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    public Theme[] Themes { get; private set; }
    public int SelectedThemeIndex { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        

        Themes = new Theme[]
        {
            new Theme(0, "Classic", "x_red", "o_blue"),
            new Theme(1, "Neon", "x_green", "o_purple"),
            new Theme(2, "Monochrome", "x_black", "o_yellow")
        };

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
}

[System.Serializable]
public class Theme
{
    public int Id;
    public string Name;
    public string XSpriteName;
    public string OSpriteName;

    public Theme(int id, string name, string xSpriteName, string oSpriteName)
    {
        Id = id;
        Name = name;
        XSpriteName = xSpriteName;
        OSpriteName = oSpriteName;
    }
}