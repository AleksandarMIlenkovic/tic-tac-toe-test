using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    private void Awake()
    {
        EnsureManagers();
    }

    public static void EnsureManagers()
    {
        var managerHolder = GameObject.Find("_Managers");
        if (managerHolder == null)
        {
            managerHolder = new GameObject("_Managers");
            DontDestroyOnLoad(managerHolder);
        }

        if (GameManager.Instance == null)
            managerHolder.AddComponent<GameManager>();

        if (StatsManager.Instance == null)
            managerHolder.AddComponent<StatsManager>();

        if (SettingsManager.Instance == null)
            managerHolder.AddComponent<SettingsManager>();

        if (ThemeManager.Instance == null)
            managerHolder.AddComponent<ThemeManager>();

        if (AudioManager.Instance == null)
        {
            var audioManager = managerHolder.AddComponent<AudioManager>();
            var audioManagerScript = audioManager.GetComponent<AudioManager>();
            if (audioManagerScript != null)
            {
                audioManagerScript.SetBGMVolume(0.5f);
                audioManagerScript.SetSFXVolume(0.7f);
            }
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateBGMState();
    }
}
