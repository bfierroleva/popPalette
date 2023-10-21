using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenomeManager
{
    private GenomeManager() { }

    private static GenomeManager instance;

    public static GenomeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GenomeManager();
            }
            return instance;
        }
    }

    public void OnStart()
    {
        _numOfMutations = 0;
        _sumOfMutations = 0;
    }

    private int _numOfMutations;
    private int _sumOfMutations;

    public Locus MutateAlleles(Locus locus, float MutationChance)
    {
        
        if(Random.value <= MutationChance)
        {
            int i = locus.AlleleValues.x;
            locus.AlleleValues.x = Mathf.Clamp(locus.AlleleValues.x + Random.Range(-locus.MutationMaxRange, locus.MutationMaxRange + 1), locus.AlleleValuesRange.x, locus.AlleleValuesRange.y + 1);
            _sumOfMutations += -1 * (i - locus.AlleleValues.x);
            _numOfMutations++;
            Debug.LogWarning(_sumOfMutations / _numOfMutations);
        }
        if (Random.value <= MutationChance)
        {
            locus.AlleleValues.y = Mathf.Clamp(locus.AlleleValues.y + Random.Range(-locus.MutationMaxRange, locus.MutationMaxRange + 1), locus.AlleleValuesRange.x, locus.AlleleValuesRange.y + 1);
        }
        
        return locus;
    } 

    public Genotype MergeReproduction(Genotype child, Genotype p1, Genotype p2) // No Atomic reproduction
    {
        return child;
    }

    public Genotype SexualBasicReproduction(Genotype child, Genotype p1, Genotype p2) // No Recombination
    {
        int i = 0;
        int j = 0;
        foreach (ChromosomePair chromosomePair in child.Karyotype)
        {
            j = 0;
            foreach (Locus locus in chromosomePair.LocusList)
            {
                child.Karyotype[i].LocusList[j].AlleleValues.x = p1.Karyotype[i].LocusList[j].AlleleValues[Random.Range(0, 2)];
                child.Karyotype[i].LocusList[j].AlleleValues.y = p2.Karyotype[i].LocusList[j].AlleleValues[Random.Range(0, 2)];
                
                Locus mutatedLocus = MutateAlleles(child.Karyotype[i].LocusList[j], child.Karyotype[i].LocusList[j].MutationChance);
                child.Karyotype[i].LocusList[j].AlleleValues.x = mutatedLocus.AlleleValues.x;
                child.Karyotype[i].LocusList[j].AlleleValues.y = mutatedLocus.AlleleValues.y;
                
                j++;
            }
            
            i++;
        }
        return child;
    }

    public Genotype SexualWithRecombinationReproduction(Genotype child, Genotype p1, Genotype p2) 
    {
        return child;
    }

}

public class Identity
{
    public int ID;
    public int Parent1ID;
    public int Parent2ID;
    public Genotype ThisGenotype;
}

public class Genotype
{
    public int HaploidNumber;
    public List<ChromosomePair> Karyotype = new List<ChromosomePair>();
}

public class ChromosomePair
{
    public int Index;
    public List<Locus> LocusList = new List<Locus>();
}

public class Locus
{
    public string Key;
    public bool IsFunctional;
    public float MutationChance;
    public int MutationMaxRange;
    public int ChromosomeIndex;
    public Vector2Int AlleleValues;
    public Vector2Int AlleleValuesRange;
}

