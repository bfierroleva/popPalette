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
    public Vector2Int AlleleValues;
    public Vector2Int AlleleValuesRange;
}


public enum TypeOfReproduction
{
    Merge,
    SexualBasic,
    SexualWithRecombination,
}