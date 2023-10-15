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

    public void MergeReproduction(Genotype child, Genotype p1, Genotype p2) // No Atomic reproduction
    {

    }

    public void SexualBasicReproduction(Genotype child, Genotype p1, Genotype p2) // No Recombination
    {
        int i = 0;
        foreach (List<Chromosome> chromosomePair in child.Karyotype)
        {
            child.Karyotype[i][0] = p1.Karyotype[i][Random.Range(0, 2)];
            child.Karyotype[i][1] = p2.Karyotype[i][Random.Range(0, 2)];
            i++;
        }
    }

    public void SexualWithRecombinationReproduction(Genotype child, Genotype p1, Genotype p2) 
    {

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
    public List<List<Chromosome>> Karyotype;
}

public class Chromosome
{
    public int Index;
    public List<Gene> GeneList;
}

public class Gene
{
    public string Name;
    public float AleleValue;
}

public enum TypeOfReproduction
{
    Merge,
    SexualBasic,
    SexualWithRecombination,
}