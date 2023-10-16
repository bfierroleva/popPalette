using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Organism : MonoBehaviour
{
    [SerializeField] private FirstGenCreator _firstGenAttributes;
    [SerializeField] private bool _isFirstGen;

    public SpeciesSO SpeciesAttributes;

    public Identity ThisIdentity;
    private IdentitiesManager _identityManager;
    private Identity mateIdentity;

    private Vector3 targetPosition;

    private void Awake()
    {
        _firstGenAttributes = transform.GetComponent<FirstGenCreator>();
        SpeciesAttributes = _firstGenAttributes.Species;
    }

    private void Start()
    {
        _identityManager = FindObjectOfType<IdentitiesManager>();     

        if (!_isFirstGen) { Destroy(transform.GetComponent<FirstGenCreator>()); }
        else WhenFirstGen();
        
        SetNewRandomDestination();
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(SpeciesAttributes.LifespanInSeconds);
    }

    void Update()
    {
        RandomMove();
    }

    void RandomMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, SpeciesAttributes.Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) SetNewRandomDestination();
    }
    void SetNewRandomDestination()
    {
        float distance = Random.Range(SpeciesAttributes.DistanceRange.x, SpeciesAttributes.DistanceRange.y);
        float angle = Random.Range(0, 360);
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);
        targetPosition = new Vector2(transform.position.x + x * distance, transform.position.y + y * distance);
    }
    private void WhenFirstGen()
    {
        ThisIdentity = _identityManager.CreateNewIdentity();
        ThisIdentity.ThisGenotype = new Genotype();
        ThisIdentity.ThisGenotype.HaploidNumber = _firstGenAttributes.Species.HaploidNumber;
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

    public void Reproduce(Organism mate)
    {
        if (mate.ThisIdentity.ID != ThisIdentity.ID) mateIdentity = mate.ThisIdentity; 
        else Debug.Log("TRIED TO MATE WITH ITSELF (WTF)");
        CreateChild();
    }

    void CreateChild()
    {
        Identity childIdentity = _identityManager.CreateNewIdentity();
        childIdentity.Parent1ID = ThisIdentity.ID;
        childIdentity.Parent2ID = mateIdentity.ID;
        switch (SpeciesAttributes.TypeOfReproduction)
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


