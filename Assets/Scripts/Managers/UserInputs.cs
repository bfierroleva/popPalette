using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInputs : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 2;
        GenomeManager.Instance.OnStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GUIUtility.systemCopyBuffer = GetComponent<SavePopData>().CollectPopData();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.timeScale < 10) Time.timeScale += 1;
            else Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Time.timeScale > 1) Time.timeScale -= 1;
            else Time.timeScale = 10;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
