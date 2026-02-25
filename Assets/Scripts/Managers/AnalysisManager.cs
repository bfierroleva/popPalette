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
    public bool DoAnalyzePhenotypeMean;

    public List<List<AnalyzedGenotype>> AnalysedGenotypeFreqData = new List<List<AnalyzedGenotype>>(); //each list a generation
    public List<List<AnalyzedAllele>> AnalysedAlleleFreqData = new List<List<AnalyzedAllele>>(); //each list a generation
    public List<List<AnalyzedPhenotype>> AnalysedPhenotypeData = new List<List<AnalyzedPhenotype>>(); // list of generations, which are list of the values for each population

    void Start()
    {
        _populationManager = GetComponent<PopulationManager>();
        _populationManager.GetComponent<GenerationManager>().NewGenerationCycle += AnalyzeGeneration;
    }

    void AnalyzeGeneration()
    {
        CurrentGeneration = _populationManager.GetComponent<GenerationManager>().CurrentGen;

        if (DoAnalyzeGenotypeFreq)
        {
            foreach (AnalyzedGenotype genotype in AnalysedGenotypeFreqData[CurrentGeneration])
            {
                //Debug.Log(genotype.FrequencyAbs);
                int pop1 = 0; //pop = numer of organisims in the population

                foreach (List<PopulationGen> population in _populationManager.PopulationGenList)
                {
                    if (population[0].PopKey == genotype.PopKey) pop1 = population[genotype.Generation].OrganismList.Count;
                }
                genotype.FrequencyRel = (float)genotype.FrequencyAbs / (float)pop1;
            }
        }
        
        if (DoAnalyzeAlleleFreq)
        {
            foreach (AnalyzedAllele allele in AnalysedAlleleFreqData[CurrentGeneration])
            {
                //Debug.Log(allele.FrequencyAbs);
                int pop2 = 0;

                foreach (List<PopulationGen> population in _populationManager.PopulationGenList)
                {
                    if (population[0].PopKey == allele.PopKey) pop2 = population[allele.Generation].OrganismList.Count;
                }
                allele.FrequencyRel = (float)allele.FrequencyAbs / ((float)pop2 * 2); //times two because every organism has 2 alleles
            }
        }
        if (DoAnalyzePhenotypeMean)
        {
            foreach (AnalyzedPhenotype _population in AnalysedPhenotypeData[CurrentGeneration])
            {
                //Debug.Log(allele.FrequencyAbs);
                int pop3 = 0;

                foreach (List<PopulationGen> population in _populationManager.PopulationGenList)
                {
                    if (population[0].PopKey == _population.PopKey) pop3 = population[_population.Generation].OrganismList.Count;
                }
                _population.PhenotypeNormalizedValue.x = (float)_population.SummedValue.x / ((float)pop3);
                _population.PhenotypeNormalizedValue.y = (float)_population.SummedValue.y / ((float)pop3);
                _population.PhenotypeNormalizedValue.z = (float)_population.SummedValue.z / ((float)pop3);
            }
        }
        

        AnalysisCompleted?.Invoke();
    }

    void AnalyzeAlleleFrequencies(AnalyzedGenotype genotype, int generation)
    {
        if (!DoAnalyzeAlleleFreq) { return; }

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

    public void AnalyzePhenotype(Phenotype phenotype, string popKey, int generation)
    {
        if (!DoAnalyzePhenotypeMean) { return; }

        if (AnalysedPhenotypeData.Count - 1 < generation) AnalysedPhenotypeData.Add(new List<AnalyzedPhenotype>());

        bool hasFoundPopulation = false;
        foreach (AnalyzedPhenotype population in AnalysedPhenotypeData[generation])
        {
            if (population.PopKey == popKey) hasFoundPopulation = true;
            else continue;

            population.SummedValue.x += phenotype.PhenotypeColor.r;
            population.SummedValue.y += phenotype.PhenotypeColor.g;
            population.SummedValue.z += phenotype.PhenotypeColor.b;

            break;
        }

        if (!hasFoundPopulation)
        {
            AnalyzedPhenotype newPhenotypeEntry = new AnalyzedPhenotype();
            newPhenotypeEntry.Generation = generation;
            newPhenotypeEntry.PopKey = popKey;
            newPhenotypeEntry.SummedValue = new Vector3(phenotype.PhenotypeColor.r, phenotype.PhenotypeColor.g, phenotype.PhenotypeColor.b);

            AnalysedPhenotypeData[generation].Add(newPhenotypeEntry);
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

public class AnalyzedPhenotype
{
    public int Generation;
    public string PopKey;
    public Vector3 SummedValue;
    public Vector3 PhenotypeNormalizedValue;
}