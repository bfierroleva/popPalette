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
        GetComponent<FirstGenManager>().OnFirstGenCreated += OnFirstGen;
    }

    void OnFirstGen()
    {
        StartCoroutine(GenerationCycle());
    }

    IEnumerator GenerationCycle()
    {
        //Debug.Log("Gen: " + CurrentGen);
        NewGenerationCycle?.Invoke();

        yield return new WaitForSeconds(LifeTime);

        CurrentGen++;      
        StartCoroutine(GenerationCycle());
    }
}
