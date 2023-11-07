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
    public int CurrentGeneration;

    public event Action AnalysisCompleted;

    public bool DoAnalyzeAlleleFreq;
    public bool DoAnalyzeGenotypeFreq;

    public List<List<AnalyzedGenotype>> AnalysedGenotypeFreqData = new List<List<AnalyzedGenotype>>(); //each list a generation
    public List<List<AnalyzedAllele>> AnalysedAlleleFreqData = new List<List<AnalyzedAllele>>(); //each list a generation

    void Start()
    {
        _populationManager = GetComponent<PopulationManager>();
        _populationManager.GetComponent<GenerationManager>().NewGenerationCycle += AnalyzeGeneration;
    }

    void AnalyzeGeneration()
    {
        CurrentGeneration = _populationManager.GetComponent<GenerationManager>().CurrentGen;

        foreach (AnalyzedGenotype genotype in AnalysedGenotypeFreqData[CurrentGeneration])
        {
            //Debug.Log(genotype.FrequencyAbs);
            int pop = 0;

            foreach (List<PopulationGen> population in _populationManager.PopulationGenList)
            {
                if (population[0].PopKey == genotype.PopKey) pop = population[genotype.Generation].OrganismList.Count;
            }
            genotype.FrequencyRel = (float)genotype.FrequencyAbs / (float)pop;
        }
        foreach (AnalyzedAllele allele in AnalysedAlleleFreqData[CurrentGeneration])
        {
            //Debug.Log(allele.FrequencyAbs);
            int pop = 0;

            foreach (List<PopulationGen> population in _populationManager.PopulationGenList)
            {
                if (population[0].PopKey == allele.PopKey) pop = population[allele.Generation].OrganismList.Count;
            }
            allele.FrequencyRel = (float)allele.FrequencyAbs / ((float)pop * 2); //times two because every organism has 2 alleles
        }
        AnalysisCompleted?.Invoke();
    }

    void AnalyzeAlleleFrequencies(AnalyzedGenotype genotype, int generation)
    {
        if (AnalysedAlleleFreqData.Count - 1 < generation) AnalysedAlleleFreqData.Add(new List<AnalyzedAllele>());

        bool foundX = false;
        bool foundY = false;
        bool XequalsY = false;

        foreach (AnalyzedAllele allele in AnalysedAlleleFreqData[generation])
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
            AnalysedAlleleFreqData[genotype.Generation].Add(newAlleleEntry);
        }
        if (!foundY && !XequalsY)
        {
            AnalyzedAllele newAlleleEntry = new AnalyzedAllele();
            newAlleleEntry.Generation = genotype.Generation;
            newAlleleEntry.LocusKey = genotype.LocusKey;
            newAlleleEntry.PopKey = genotype.PopKey;
            newAlleleEntry.AlleleValue = genotype.AlleleValues.y;
            newAlleleEntry.FrequencyAbs = 1;
            AnalysedAlleleFreqData[genotype.Generation].Add(newAlleleEntry);
        }
    }

    public void AnalyzeGenotype(Vector2Int alleleValues, string locusKey, string popKey, int generation)
    {
        if (!DoAnalyzeGenotypeFreq) { return; }

        if (AnalysedGenotypeFreqData.Count - 1 < generation) AnalysedGenotypeFreqData.Add(new List<AnalyzedGenotype>());
        bool hasFoundGenotype = false;
        foreach (AnalyzedGenotype genotype in AnalysedGenotypeFreqData[generation])
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
            AnalysedGenotypeFreqData[generation].Add(newGenotypeEntry);
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
    public float FrequencyRel;
}
public class AnalyzedAllele
{
    public int Generation;
    public string LocusKey;
    public string PopKey;
    public int AlleleValue;
    public float FrequencyAbs;
    public float FrequencyRel;
}