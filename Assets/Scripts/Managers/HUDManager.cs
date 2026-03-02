using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HUDManager : MonoBehaviour
{

    private UserInputs _userInputs;

    private void Start()
    {
        _userInputs = FindObjectOfType<UserInputs>();
    }
 
    public void PressStart()
    {
        _userInputs.StartSim();
    }

    public void PressReStart()
    {
        _userInputs.ReStartSim();
    }

    public void PressPause()
    {
        _userInputs.Pause();
    }

    public void PressChangeSpeed(bool pos)
    {
        _userInputs.ChangeSpeed(pos);
    }

    public void PressSavePopData()
    {
        _userInputs.SavePopDataToClipBoard();
    }
}
