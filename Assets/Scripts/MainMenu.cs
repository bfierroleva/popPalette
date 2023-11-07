using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MainMenu : MonoBehaviour
{
    public string SpanishUserManualLink = "https://www.youtube.com/watch?v=BpeXRSi8fvw";
    public string EnglishUserManualLink = "https://www.youtube.com/watch?v=BpeXRSi8fvw";

    public void OpenUserManual()
    {
        if (LocalizationSettings.SelectedLocale.LocaleName == "Spanish (es)") Application.OpenURL(SpanishUserManualLink); 
        else if (LocalizationSettings.SelectedLocale.LocaleName == "English (en)") Application.OpenURL(EnglishUserManualLink);
    }
}
