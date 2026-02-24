using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.PlayerSettings;

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

    public Locus MutateMicrosatellite(Locus locus)
    {
        float random = Random.value;
        if (random <= MutationChance/2)
        {
            int i = locus.AlleleValues.x;
            locus.AlleleValues.x = Mathf.Clamp(locus.AlleleValues.x + Random.Range(-locus.MutationMaxRange, locus.MutationMaxRange + 1), locus.AlleleValuesRange.x, locus.AlleleValuesRange.y + 1);
            _sumOfMutations += -1 * (i - locus.AlleleValues.x);
            _numOfMutations++;
            //Debug.LogWarning(_sumOfMutations / _numOfMutations);
        }
        else if (random <= MutationChance)
        {
            locus.AlleleValues.y = Mathf.Clamp(locus.AlleleValues.y + Random.Range(-locus.MutationMaxRange, locus.MutationMaxRange + 1), locus.AlleleValuesRange.x, locus.AlleleValuesRange.y + 1);
        }
        
        return locus;
    }

    public string MutateNonCodingSequence(Locus locus, int copyIndex)
    {
        //using Poisson approximation:

        /*
        int length = locus.DNASequence.Length;
        float lambdaDeletion = length * locus.DeletionChance;
        float lambdaAddition = length * locus.AdditionChance;
        float lambdaTransition = length * locus.TransitionChance;
        float lambdaTransversion = length * locus.TransversionChance;

        int DeletionsNum = MutationUtils.SamplePoisson(lambdaDeletion);
        int AdditionsNum = MutationUtils.SamplePoisson(lambdaAddition);
        int TransitionsNum = MutationUtils.SamplePoisson(lambdaTransition);
        int TransversionsNum = MutationUtils.SamplePoisson(lambdaTransversion);
        */

        //using real mutations:

        List<char> sequence = new List<char>(locus.DNASequence[copyIndex]);

        int i = 0;

        while (i < sequence.Count)
        {
            float r = Random.value;

            float pDel = locus.DeletionChance;
            float pTra = locus.TransitionChance;
            float pTrv = locus.TransversionChance;

            float totalP = pDel + pTra + pTrv;

            if (r < totalP)
            {
                float x = Random.value * totalP;

                if (x < pDel)
                {
                    // Deletion
                    sequence.RemoveAt(i);
                    continue; // no increment i
                }
                else if (x < pDel + pTra)
                {
                    // Transition
                    sequence[i] = Transition(sequence[i]);
                }
                else
                {
                    // Transversion
                    sequence[i] = Transversion(sequence[i]);
                }
            }

            i++;
        }

        // Inserciones (no ligadas a bases existentes)
        int insertions = MutationUtils.SamplePoisson(sequence.Count * locus.AdditionChance);

        for (int j = 0; j < insertions; j++)
        {
            int pos = Random.Range(0, sequence.Count + 1);
            sequence.Insert(pos, Addition());
        }

        string newSequence = new string(sequence.ToArray());
        return newSequence;
    }

    public char Addition()
    {
        char[] bases = { 'A', 'C', 'G', 'T' };
        return bases[Random.Range(0, bases.Length)];
    }

    public char Transition(char baseToMutate) 
    {
        char mutatedBase = baseToMutate;
        switch (baseToMutate)
        {
            case 'A':
                mutatedBase = 'G';
                break;

            case 'T':
                mutatedBase = 'C';
                break;

            case 'C':
                mutatedBase = 'T';
                break;
            case 'G':
                mutatedBase = 'A';
                break;
        }
        return mutatedBase;
    }

    public char Transversion(char baseToMutate)
    {
        char[] options;
        switch (baseToMutate)
        {
            case 'A': options = new char[] { 'C', 'T' }; break;
            case 'G': options = new char[] { 'C', 'T' }; break;
            case 'C': options = new char[] { 'A', 'G' }; break;
            case 'T': options = new char[] { 'A', 'G' }; break;
            default: return baseToMutate;
        }
        return options[Random.Range(0, options.Length)];
    }

    public Genotype MergeReproduction(Genotype child, Genotype p1, Genotype p2) // No Atomic reproduction
    {
        int i = 0;
        int j = 0;
        foreach (ChromosomePair chromosomePair in child.Karyotype)
        {
            j = 0;
            foreach (Locus locus in chromosomePair.LocusList)
            {
                child.Karyotype[i].LocusList[j].AlleleValues.x = p1.Karyotype[i].LocusList[j].AlleleValues[0];
                child.Karyotype[i].LocusList[j].AlleleValues.y = p2.Karyotype[i].LocusList[j].AlleleValues[0];

                Locus mutatedLocus = MutateMicrosatellite(child.Karyotype[i].LocusList[j]);
                child.Karyotype[i].LocusList[j].AlleleValues.x = mutatedLocus.AlleleValues.x;
                child.Karyotype[i].LocusList[j].AlleleValues.y = mutatedLocus.AlleleValues.y;


                if (child.Karyotype[i].LocusList[j].AlleleValues.x != child.Karyotype[i].LocusList[j].AlleleValues.y)
                {
                    int meanAllele = Mathf.RoundToInt(((float)child.Karyotype[i].LocusList[j].AlleleValues.x + (float)child.Karyotype[i].LocusList[j].AlleleValues.y) / 2f);
                    child.Karyotype[i].LocusList[j].AlleleValues.x = meanAllele;
                    child.Karyotype[i].LocusList[j].AlleleValues.y = meanAllele;
                }


                j++;
            }

            i++;
        }
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
                switch (locus.Type)
                {
                    case LocusType.Microsatellite:

                        locus.AlleleValues.x = p1.Karyotype[i].LocusList[j].AlleleValues[Random.Range(0, 2)];
                        locus.AlleleValues.y = p2.Karyotype[i].LocusList[j].AlleleValues[Random.Range(0, 2)];

                        Locus mutatedLocus = MutateMicrosatellite(child.Karyotype[i].LocusList[j]);

                        locus.AlleleValues.x = mutatedLocus.AlleleValues.x;
                        locus.AlleleValues.y = mutatedLocus.AlleleValues.y;


                        if (locus.AlleleValues.x < locus.AlleleValues.y)
                        {
                            int xAllele = locus.AlleleValues.x;
                            locus.AlleleValues.x = locus.AlleleValues.y;
                            locus.AlleleValues.y = xAllele;
                        }

                        break;

                    case LocusType.CodingGene:
                        // to be worked on in the future
                        break;

                    case LocusType.NonCodingRegion:

                        locus.DNASequence[0] = p1.Karyotype[i].LocusList[j].DNASequence[Random.Range(0, 2)];
                        locus.DNASequence[1] = p2.Karyotype[i].LocusList[j].DNASequence[Random.Range(0, 2)];

                        locus.DNASequence[0] = MutateNonCodingSequence(locus, 0);
                        locus.DNASequence[1] = MutateNonCodingSequence(locus, 1);

                        break;
                }

                

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
    public int Generation;
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
    public LocusType Type;
    public string Key;
    public int ChromosomeIndex;

    // for genes
    public string[] DNASequence = new string[2];
    public float DeletionChance;
    public float AdditionChance;
    public float TransitionChance;
    public float TransversionChance;

    public List<char> Codons;       // si es codificante

    public Vector2Int AlleleValues; // solo si es microsat�lite
    public bool IsFunctional;
    public float MutationChance;
    public int MutationMaxRange;
    
    public Vector2Int AlleleValuesRange;
}

public enum LocusType
{
    Microsatellite,
    CodingGene,
    NonCodingRegion
}

public static class MutationUtils
{
    public static int SamplePoisson(float lambda)
    {
        if (lambda <= 0f)
            return 0;

        float L = Mathf.Exp(-lambda);
        int k = 0;
        float p = 1f;

        do
        {
            k++;
            p *= Random.value;
        }
        while (p > L);

        return k - 1;
    }
}