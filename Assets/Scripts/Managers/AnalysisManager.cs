using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalysisManager : MonoBehaviour
{
    [SerializeField] private float _intervalTimeInSimSeconds;
    private int _timesAnalyzed;
    private PopulationManager _populationManager;
    void Start()
    {
        _populationManager = GetComponent<PopulationManager>();
        StartCoroutine(AnalyzeFrequencies());

    }

    IEnumerator AnalyzeFrequencies()
    {
        yield return new WaitForSeconds(_intervalTimeInSimSeconds);
        _timesAnalyzed++;
        float frequencyDrift = (_populationManager.highRedFrequency + _populationManager.medRedFrequency / 2) - (_populationManager.lowRedFrequency + _populationManager.medRedFrequency / 2);
        //Debug.Log(_timesAnalyzed * _intervalTimeInSimSeconds + "s : " + frequencyDrift);
        if(_timesAnalyzed == 10) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine(AnalyzeFrequencies());

    }
}
