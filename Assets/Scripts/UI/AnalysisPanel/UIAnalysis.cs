using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnalysis : MonoBehaviour
{
    private AnalysisManager _anaAnalysisManager;
    private PopulationManager _populationManager;

    [SerializeField] private bool _doShowGenotypeFreq;
    [SerializeField] private bool _doShowAlleleFreq;

    [SerializeField] private List<UIGenotypeFreqOutput> GenotypeFreqOutputPanel;
    [SerializeField] private List<UIAlleleFreqOutput> AlleleFreqOutputPanel;

    private void Start()
    {
        _anaAnalysisManager = FindObjectOfType<AnalysisManager>();
        _populationManager = _anaAnalysisManager.GetComponent<PopulationManager>();

        _anaAnalysisManager.AnalysisCompleted += OnAnalysisCompleted;
    }

    void OnAnalysisCompleted()
    {
        if (_doShowGenotypeFreq) ShowGenotypeFreq();
        if (_doShowAlleleFreq) ShowAlleleFreq();
    }

    void ShowGenotypeFreq()
    {
        foreach (UIGenotypeFreqOutput item in GenotypeFreqOutputPanel)
        {
            item.ShowData(_anaAnalysisManager.AnalysedGenotypeFreqData[_anaAnalysisManager.CurrentGeneration]);
        }
        
    }

    void ShowAlleleFreq()
    {
        foreach (UIAlleleFreqOutput item in AlleleFreqOutputPanel)
        {
            item.ShowData(_anaAnalysisManager.AnalysedAlleleFreqData[_anaAnalysisManager.CurrentGeneration]);
        }

    }
}
