using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

//refactorizar para varias espcecies
public class GenerationManager : MonoBehaviour
{
    public float LifeTime = 0;
    public List<int> GenerationPopCount = new List<int>();
    public int CurrentGen = 0;
    public event Action NewGenerationCycle;

    private void Start()
    {
        GenerationPopCount.Add(0);
        GetComponent<UserInputs>().OnStart += OnStart;
    }

    void OnStart()
    {
        NewGenerationCycle?.Invoke();
        StartCoroutine(GenerationCycle(1));
    }

    IEnumerator GenerationCycle(float i)
    {
        yield return new WaitForSeconds(i);
        yield return new WaitForSeconds(LifeTime);
        CurrentGen++;
        Debug.Log("Gen: " + CurrentGen);
        NewGenerationCycle?.Invoke();
        StartCoroutine(GenerationCycle(0));
    }
}
