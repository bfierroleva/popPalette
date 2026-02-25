using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public string SpanishUserManualLink = "";
    public string EnglishUserManualLink = "";
    public bool isManager;
    public bool isToggleScreen;

    int CurrentRes;
    bool isFullScreen;


    private void Start()
    {
        if(!isManager && !isToggleScreen) { return; }

        if (isManager)
        {
            SetResolution(1920);
            isFullScreen = false;
            ToggleFullscreen();
        }else if ( isToggleScreen)
        {
            isFullScreen = true;
        }

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

    public void ToggleFullscreen()
    {
        if (isFullScreen)
        {
            Debug.Log("isFullscreen: " + isFullScreen);
            Screen.fullScreenMode = FullScreenMode.Windowed;
            isFullScreen = false;
            Debug.Log("set windowed");
        }
        else if (!isFullScreen)
        {
            Debug.Log("isFullscreen: " + isFullScreen);
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            isFullScreen = true;
            Debug.Log("set fullscreen");
        }
        Debug.Log("isFullscreen set to: " + isFullScreen);
    }

    public void SetResolution(int x)
    {
        Screen.SetResolution(x, Mathf.RoundToInt(x * (9f / 16f)), Screen.fullScreenMode);
        CurrentRes = x;
    }
}
