using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Organism : MonoBehaviour
{
    [SerializeField] private FirstGenCreator _firstGenAttributes;
    [SerializeField] private bool _isFirstGen;

    private TypeOfReproduction _typeOfReproduction;
    private TypeOfReproduction _speciesName;

    public Identity ThisIdentity;
    private IdentitiesManager _identityManager;
    public bool _isReadyToMate;
    private Identity mateIdentity;

    private void Start()
    {
        _identityManager = FindObjectOfType<IdentitiesManager>();

        if (!_isFirstGen) { Destroy(transform.GetComponent<FirstGenCreator>()); }
        else WhenFirstGen();
        foreach (ChromosomePair pair in ThisIdentity.ThisGenotype.Karyotype)
        {
            foreach (Locus locus in pair.LocusList)
            {
                Debug.Log(locus.Key);
                Debug.Log("x : " + locus.AlleleValues.x);
                Debug.Log("y : " + locus.AlleleValues.y);
            }            
        }    
    }

    private void WhenFirstGen()
    {
        _firstGenAttributes = transform.GetComponent<FirstGenCreator>();
        _typeOfReproduction = _firstGenAttributes.TypeOfReproduction;
        ThisIdentity = _identityManager.CreateNewIdentity();
        ThisIdentity.ThisGenotype = new Genotype();
        ThisIdentity.ThisGenotype.HaploidNumber = _firstGenAttributes.HaploidNumber;
        ThisIdentity.ThisGenotype.Karyotype = new List<ChromosomePair>();
        int i = 0;
        while (i < ThisIdentity.ThisGenotype.HaploidNumber) { ThisIdentity.ThisGenotype.Karyotype.Add(new ChromosomePair()); i++; }
        int j = 0;
        foreach (ChromosomePair chromosomePair in ThisIdentity.ThisGenotype.Karyotype)
        {            
            foreach (LocusSO locus in _firstGenAttributes.LocusList)
            {               
                if (locus.ChromosomeIndex == j) 
                {
                    Locus newLocus = new Locus();
                    newLocus.Key = locus.Key;
                    newLocus.AlleleValues = locus.AlleleValues;
                    chromosomePair.LocusList.Add(newLocus); 
                }
            }
            j++;
        }
    }

    void Reproduce()
    {
        foreach (Organism possibleMate in _identityManager.LivingOrganisms)
        {
            if (possibleMate.ThisIdentity.ID != ThisIdentity.ID && possibleMate._isReadyToMate) mateIdentity = possibleMate.ThisIdentity; break;
        }
        CreateChild();
    }

    void CreateChild()
    {
        Identity childIdentity = _identityManager.CreateNewIdentity();
        childIdentity.Parent1ID = ThisIdentity.ID;
        childIdentity.Parent2ID = mateIdentity.ID;
        switch (_typeOfReproduction)
        {
            case TypeOfReproduction.Merge:
                GenomeManager.Instance.MergeReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, mateIdentity.ThisGenotype);
                break;

            case TypeOfReproduction.SexualBasic:
                GenomeManager.Instance.SexualBasicReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, mateIdentity.ThisGenotype);
                break;

            case TypeOfReproduction.SexualWithRecombination:
                GenomeManager.Instance.SexualWithRecombinationReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, mateIdentity.ThisGenotype);
                break;

            default:
                GenomeManager.Instance.SexualBasicReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, mateIdentity.ThisGenotype);
                break;
        }
        
    }

}


