using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnalysis : MonoBehaviour
{
    private AnalysisManager _anaAnalysisManager;
    private PopulationManager _populationManager;

    //[SerializeField] private bool _doShowGenotypeFreq;
    //[SerializeField] private bool _doShowAlleleFreq;
    //[SerializeField] private bool _doShowPhenotypeMean;

    [SerializeField] private List<UIGenotypeFreqOutput> GenotypeFreqOutputPanel;
    [SerializeField] private List<UIAlleleFreqOutput> AlleleFreqOutputPanel;
    [SerializeField] private List<UIPhenotypeOutput> PhenotypeOutputPanel;

    private void Start()
    {
        _anaAnalysisManager = FindObjectOfType<AnalysisManager>();
        _populationManager = _anaAnalysisManager.GetComponent<PopulationManager>();

        _anaAnalysisManager.AnalysisCompleted += OnAnalysisCompleted;
    }

    void OnAnalysisCompleted()
    {
        if (_anaAnalysisManager.DoAnalyzeGenotypeFreq) ShowGenotypeFreq();
        if (_anaAnalysisManager.DoAnalyzeAlleleFreq) ShowAlleleFreq();
        if (_anaAnalysisManager.DoAnalyzePhenotypeMean) ShowPhenotype();
    }

    void ShowGenotypeFreq()
    {
        foreach (UIGenotypeFreqOutput item in GenotypeFreqOutputPanel)
        {
            item.ShowData(_anaAnalysisManager.AnalysedGenotypeFreqData[_anaAnalysisManager.CurrentGeneration]);
        }
        
    }

    void ShowPhenotype()
    {
        foreach (UIPhenotypeOutput item in PhenotypeOutputPanel)
        {
            item.ShowData(_anaAnalysisManager.AnalysedPhenotypeData[_anaAnalysisManager.CurrentGeneration]);
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
