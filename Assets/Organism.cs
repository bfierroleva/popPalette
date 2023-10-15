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

        if (!_isFirstGen) { Destroy(transform.GetComponent<FirstGenCreator>()); return; }
        _typeOfReproduction = _firstGenAttributes.TypeOfReproduction;
        ThisIdentity = _identityManager.CreateNewIdentity();
        ThisIdentity.ThisGenotype = new Genotype();
        ThisIdentity.ThisGenotype.HaploidNumber = _firstGenAttributes.HaploidNumber;
        ThisIdentity.ThisGenotype.Karyotype = new List<List<Chromosome>>();
        int i = 0;
        while (i < ThisIdentity.ThisGenotype.HaploidNumber) { ThisIdentity.ThisGenotype.Karyotype.Add(new List<Chromosome>()); i++; }
        int j = 0;
        foreach (List<Chromosome> chromosomePair in ThisIdentity.ThisGenotype.Karyotype)
        {
            Chromosome c1 = new Chromosome();
            Chromosome c2 = new Chromosome();
            c1.GeneList.AddRange(_firstGenAttributes.ChromosomePairList[j][0]);
            c2.GeneList.AddRange(_firstGenAttributes.ChromosomePairList[j][1]);

            ThisIdentity.ThisGenotype.Karyotype[j][0] = c1;
            ThisIdentity.ThisGenotype.Karyotype[j][1] = c2;
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


