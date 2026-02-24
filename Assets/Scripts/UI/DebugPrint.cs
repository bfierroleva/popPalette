using UnityEngine;
using TMPro;
using System.Text;

public class BuildDebugConsole : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private StringBuilder logBuilder = new StringBuilder();
    private int maxLines = 7;
    int i = 0;
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {

        logBuilder.AppendLine(i + ": " + logString);
        i++;
        // Optional: limit lines
        string[] lines = logBuilder.ToString().Split('\n');
        if (lines.Length > maxLines)
        {
            logBuilder.Clear();
            for (int i = lines.Length - maxLines; i < lines.Length; i++)
            {
                logBuilder.AppendLine(lines[i]);
            }
        }

        debugText.text = logBuilder.ToString();
    }
}