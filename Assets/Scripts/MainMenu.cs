using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public string SpanishUserManualLink = "";
    public string EnglishUserManualLink = "";
    int CurrentRes;
    bool isFullScreen;

    private void Start()
    {
        SetResolution(1920);
        SetFullscreen();

    }

    public void OpenUserManual()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "Spanish (es)") Application.OpenURL(SpanishUserManualLink);
        else if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)") Application.OpenURL(EnglishUserManualLink);
    }

    public void ChangeScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
        
    }

    public void ChangeLanguage()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "Spanish (es)")
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
        }
        else if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)") {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("es");
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        isFullScreen = true;
        Debug.Log("Modo de pantalla cambiado a: " + Screen.fullScreenMode);
    }

    public void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        isFullScreen = false;
        Debug.Log("Modo de pantalla cambiado a: " + Screen.fullScreenMode);
    }

    public void SetResolution(int x)
    {
        Screen.SetResolution(x, Mathf.RoundToInt(x * (9f / 16f)), Screen.fullScreenMode);
        CurrentRes = x;
        Debug.Log("Res cambiada a: " + Screen.currentResolution);
    }
}
