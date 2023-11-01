using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering.Universal;

public class AnalysisManager : MonoBehaviour
{
    //private int _timesAnalyzed;
    private PopulationManager _populationManager;
    private int _currentGeneration;

    public event Action AnalysisCompleted;

    public bool DoAnalyzeAlleleFreq;
    public bool DoAnalyzeGenotypeFreq;

    private List<List<AnalyzedGenotype>> _analysedGenotypeFreqData = new List<List<AnalyzedGenotype>>(); //each list a generation
    private List<List<AnalyzedAllele>> _analysedAlleleFreqData = new List<List<AnalyzedAllele>>(); //each list a generation

    void Start()
    {
        _populationManager = GetComponent<PopulationManager>();
        _populationManager.GetComponent<GenerationManager>().NewGenerationCycle += AnalyzeGeneration;
        _currentGeneration = -1;
    }

    void AnalyzeGeneration()
    {
        _currentGeneration++;

        foreach (AnalyzedGenotype genotype in _analysedGenotypeFreqData[_currentGeneration])
        {
            Debug.Log(" POP : " + genotype.PopKey + " locus : " + genotype.LocusKey + " : " + genotype.AlleleValues + " = " + genotype.FrequencyAbs);
        }
        foreach (AnalyzedAllele allele in _analysedAlleleFreqData[_currentGeneration])
        {
            Debug.Log(" POP : " + allele.PopKey + " locus : " + allele.LocusKey + " : " + allele.AlleleValue + " = " + allele.FrequencyAbs);
        }
    }

    void AnalyzeAlleleFrequencies(AnalyzedGenotype genotype, int generation)
    {
        if (_analysedAlleleFreqData.Count - 1 < generation) _analysedAlleleFreqData.Add(new List<AnalyzedAllele>());

        bool foundX = false;
        bool foundY = false;
        bool XequalsY = false;

        foreach (AnalyzedAllele allele in _analysedAlleleFreqData[generation])
        {
            if (allele.LocusKey != genotype.LocusKey) continue;
            if (allele.PopKey != genotype.PopKey) continue;
            if (allele.AlleleValue == genotype.AlleleValues.x)
            {
                foundX = true;
                allele.FrequencyAbs++;
            }
            if (allele.AlleleValue == genotype.AlleleValues.y)
            {
                foundY = true;
                allele.FrequencyAbs++;
            }
        }

        if (genotype.AlleleValues.x == genotype.AlleleValues.y) XequalsY = true;

        if (!foundX)
        {
            AnalyzedAllele newAlleleEntry = new AnalyzedAllele();
            newAlleleEntry.Generation = genotype.Generation;
            newAlleleEntry.LocusKey = genotype.LocusKey;
            newAlleleEntry.PopKey = genotype.PopKey;
            newAlleleEntry.AlleleValue = genotype.AlleleValues.x;
            newAlleleEntry.FrequencyAbs = 1;
            if (XequalsY) newAlleleEntry.FrequencyAbs++;
            _analysedAlleleFreqData[genotype.Generation].Add(newAlleleEntry);
        }
        if (!foundY && !XequalsY)
        {
            AnalyzedAllele newAlleleEntry = new AnalyzedAllele();
            newAlleleEntry.Generation = genotype.Generation;
            newAlleleEntry.LocusKey = genotype.LocusKey;
            newAlleleEntry.PopKey = genotype.PopKey;
            newAlleleEntry.AlleleValue = genotype.AlleleValues.y;
            newAlleleEntry.FrequencyAbs = 1;
            _analysedAlleleFreqData[genotype.Generation].Add(newAlleleEntry);
        }
    }

    public void AnalyzeGenotype(Vector2Int alleleValues, string locusKey, string popKey, int generation)
    {
        if (!DoAnalyzeGenotypeFreq) { return; }

        if (_analysedGenotypeFreqData.Count - 1 < generation) _analysedGenotypeFreqData.Add(new List<AnalyzedGenotype>());
        bool hasFoundGenotype = false;
        foreach (AnalyzedGenotype genotype in _analysedGenotypeFreqData[generation])
        {
            if (genotype.LocusKey == locusKey && genotype.PopKey == popKey && genotype.AlleleValues == alleleValues) hasFoundGenotype = true;
            else continue;

            genotype.FrequencyAbs++;
            if (DoAnalyzeAlleleFreq) AnalyzeAlleleFrequencies(genotype, generation);
            break;
        }

        if (!hasFoundGenotype) 
        {
            AnalyzedGenotype newGenotypeEntry = new AnalyzedGenotype();
            newGenotypeEntry.Generation = generation;
            newGenotypeEntry.LocusKey = locusKey;
            newGenotypeEntry.PopKey = popKey;
            newGenotypeEntry.AlleleValues = alleleValues;
            newGenotypeEntry.FrequencyAbs = 1;
            if (DoAnalyzeAlleleFreq) AnalyzeAlleleFrequencies(newGenotypeEntry, generation);
            _analysedGenotypeFreqData[generation].Add(newGenotypeEntry);
        }
    }
}

public class AnalyzedGenotype
{
    public int Generation;
    public string LocusKey;
    public string PopKey;
    public Vector2Int AlleleValues;
    public int FrequencyAbs;
}
public class AnalyzedAllele
{
    public int Generation;
    public string LocusKey;
    public string PopKey;
    public int AlleleValue;
    public float FrequencyAbs;
}