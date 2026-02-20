using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string SpanishUserManualLink = "";
    public string EnglishUserManualLink = "";

    public void OpenUserManual()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "Spanish (es)") Application.OpenURL(SpanishUserManualLink);
        else if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)") Application.OpenURL(EnglishUserManualLink);
    }

    public void ChangeScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);

    }
}
