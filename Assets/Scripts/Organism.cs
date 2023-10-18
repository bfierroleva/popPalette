using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Organism : MonoBehaviour
{
    [SerializeField] private FirstGenCreator _firstGenAttributes;
    [SerializeField] private bool _isFirstGen;
    public bool IsFemale;

    private GameObject _manager;
    private IdentitiesManager _identityManager;
    private MatingManager _matingManager;

    public SpeciesSO SpeciesAttributes;
    public Identity ThisIdentity;
    private Population _population;
    private Organism _mate;

    private Vector3 _targetPosition;
    private bool _isMovingToMate = false;

    private void Awake()
    {
        _firstGenAttributes = transform.GetComponent<FirstGenCreator>();
        SpeciesAttributes = _firstGenAttributes.Species;
    }

    private void Start()
    {
        _manager = FindObjectOfType<IdentitiesManager>().gameObject;
        _identityManager = _manager.GetComponent<IdentitiesManager>(); 
        _matingManager = _manager.GetComponent<MatingManager>();

        if (!_isFirstGen) { Destroy(transform.GetComponent<FirstGenCreator>()); }
        else WhenFirstGen();

        OnBirth();
    }

    public void OnBirth()
    {
        IsFemale = Random.Range(0, 2) == 0;
        GetComponent<Phenotype>().SetPhenotype(ThisIdentity);
        _population = _manager.GetComponent<PopulationManager>().AddOrganism(this, SpeciesAttributes.SpeciesKey);
        SetNewRandomDestination();
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(SpeciesAttributes.LifespanInSeconds);
        _population.PopulationList.Remove(this);
        Destroy(gameObject);
    }

    void Update()
    {    
        if (_isMovingToMate) MoveToMate();
        else RandomMove();
    }

    void RandomMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, SpeciesAttributes.Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.01f) SetNewRandomDestination();
    }
    void SetNewRandomDestination()
    {
        float distance = Random.Range(SpeciesAttributes.DistanceRange.x, SpeciesAttributes.DistanceRange.y);
        float angle = Random.Range(0, 360);
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);
        _targetPosition = new Vector2(transform.position.x + x * distance, transform.position.y + y * distance);
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
                    newLocus.IsFunctional = locus.IsFunctional;
                    newLocus.AlleleValuesRange = locus.AlleleValuesRange;
                    newLocus.AlleleValues = locus.AlleleValues;
                    newLocus.MutationMaxRange = locus.MutationMaxRange;
                    chromosomePair.LocusList.Add(newLocus); 
                }
            }
            j++;
        }
    }

    void MoveToMate()
    {
        if (_mate == null) { _isMovingToMate = false; return; }
        Vector3 direction = (_mate.transform.position - transform.position).normalized;
        transform.position += direction * SpeciesAttributes.Speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, _mate.transform.position) < 0.05f) 
        { 
            if (CanReproduce()) CreateChild(); 
            _isMovingToMate = false; 
        }
    }

    public void SetMate(Organism mate)
    {
        if (mate == null) return;
        if (mate.ThisIdentity.ID != ThisIdentity.ID) _mate = mate;
        else Debug.Log("TRIED TO MATE WITH ITSELF (WTF)");


        _isMovingToMate = true;
    }

    bool CanReproduce()
    {
        float reproductionChance = 0;
        float meanFitnessOfParents = (_manager.GetComponent<SelectivePreassure>().CalculateFitness(GetComponent<Phenotype>().PhenotypeColor) + _manager.GetComponent<SelectivePreassure>().CalculateFitness(_mate.gameObject.GetComponent<Phenotype>().PhenotypeColor)) / 2;
        //Debug.Log(" meanFitnessOfParents: " + meanFitnessOfParents);
        // alomejor cambiar  _population.PopulationList.Count por la suma de todas las poblaciones
        reproductionChance = meanFitnessOfParents * (1 - (_population.PopulationList.Count / (float)SpeciesAttributes.MaxPopulation));
        //Debug.Log(meanFitnessOfParents + " => " + reproductionChance);
        return Random.value <= reproductionChance;
    }

    void CreateChild()
    {
        GameObject child = Instantiate(SpeciesAttributes.Prefab, transform.position, Quaternion.identity, _identityManager.gameObject.transform);
        Identity childIdentity = _identityManager.CreateNewIdentity();
        childIdentity.Parent1ID = ThisIdentity.ID;
        childIdentity.Parent2ID = _mate.ThisIdentity.ID;
        childIdentity.ThisGenotype = new Genotype();
        childIdentity.ThisGenotype.Karyotype.AddRange(ThisIdentity.ThisGenotype.Karyotype);
        switch (SpeciesAttributes.TypeOfReproduction)
        {
            case TypeOfReproduction.Merge:
                childIdentity.ThisGenotype = GenomeManager.Instance.MergeReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype);
                break;

            case TypeOfReproduction.SexualBasic:
                childIdentity.ThisGenotype = GenomeManager.Instance.SexualBasicReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype, SpeciesAttributes.MutationChance);
                break;

            case TypeOfReproduction.SexualWithRecombination:
                childIdentity.ThisGenotype = GenomeManager.Instance.SexualWithRecombinationReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype);
                break;

            default:
                childIdentity.ThisGenotype = GenomeManager.Instance.SexualBasicReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype, SpeciesAttributes.MutationChance);
                break;
        }
        child.GetComponent<Organism>().ThisIdentity = childIdentity;
    }

}


