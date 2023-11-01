using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UserInputs : MonoBehaviour
{
    private float _lastTimeScale;
    private bool _hasStarted;
    public event Action OnStart;

    private void Start()
    {
        Time.timeScale = 2;
        GenomeManager.Instance.OnStart();
    }

    public void StartSim()
    {
        if (_hasStarted) return;
        OnStart?.Invoke();
        _hasStarted = true;
    }

    public void ReStartSim()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        if (Time.timeScale != 0)
        {
            _lastTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = _lastTimeScale;
        }
    }

    public void ChangeSpeed(bool pos)
    {
        if (Time.timeScale == 0) return;
        float newTime = Time.timeScale + 1;
        if (!pos) newTime = newTime * -1;
        if (newTime > 10) newTime = 1;
        else if (newTime < 1) newTime = 10;

        Time.timeScale = newTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GUIUtility.systemCopyBuffer = GetComponent<SavePopData>().GetGenData(GetComponent<GenerationManager>().CurrentGen);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeSpeed(true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeSpeed(false);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReStartSim();
        }
    }
}
