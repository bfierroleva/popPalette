using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Organism : MonoBehaviour
{
    public FirstGenCreator FirstGenAttributes;
    public bool _isFirstGen;

    private GameObject _manager;
    private IdentitiesManager _identityManager;
    private MatingManager _matingManager;

    public Phenotype Phenotype;
    public SpeciesAttributes SpeciesAttributes;
    public Identity ThisIdentity;
    private Population _population;
    private Organism _mate;

    private Vector3 _targetPosition;
    private bool _isMovingToMate = false;
    private int _timesMated;

    private float _speed;

    private void Awake()
    {
        //_firstGenAttributes = transform.GetComponent<FirstGenCreator>();
        Phenotype = GetComponent<Phenotype>();
        SpeciesAttributes = GetComponent<SpeciesAttributes>();
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

    private void WhenFirstGen()
    {
        SpeciesAttributes.Prefab.GetComponent<Organism>()._isFirstGen = false;
        ThisIdentity = _identityManager.CreateNewIdentity();
        ThisIdentity.ThisGenotype = new Genotype();
        ThisIdentity.ThisGenotype.HaploidNumber = SpeciesAttributes.HaploidNumber;
        ThisIdentity.ThisGenotype.Karyotype = new List<ChromosomePair>();
        int i = 0;
        while (i < ThisIdentity.ThisGenotype.HaploidNumber) { ThisIdentity.ThisGenotype.Karyotype.Add(new ChromosomePair()); i++; }
        int j = 0;
        foreach (ChromosomePair chromosomePair in ThisIdentity.ThisGenotype.Karyotype)
        {
            foreach (Locus locus in FirstGenAttributes.LocusList)
            {
                if (locus.ChromosomeIndex == j)
                {
                    chromosomePair.LocusList.Add(locus);
                }
            }
            j++;
        }
    }

    public void OnBirth()
    {       
        GetComponent<Phenotype>().SetPhenotype(ThisIdentity);
        _population = _manager.GetComponent<PopulationManager>().AddOrganism(this, SpeciesAttributes.SpeciesKey);
        if (!CanSurviveBirth() && !_isFirstGen) OnDeath();
        _speed = SpeciesAttributes.StartingSpeed;
        SetNewRandomDestination();
        StartCoroutine(LifeCycle());
        
    }
    IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(SpeciesAttributes.LifespanInSeconds / SpeciesAttributes.NumberOfMatingsForLifetime);
        if (_timesMated < SpeciesAttributes.NumberOfMatingsForLifetime) { LookForAMate(); StartCoroutine(LifeCycle()); _timesMated++; }
        else OnDeath();
    }

    bool CanSurviveBirth()
    {
        float survivalChance = 0;
        float fitness = _manager.GetComponent<SelectivePreassure>().CalculateFitness(Phenotype.PhenotypeColor);
        // alomejor cambiar  _population.PopulationList.Count por la suma de todas las poblaciones
        survivalChance = fitness * Mathf.Pow(1 - (_population.OrganismList.Count / (float)SpeciesAttributes.MaxPopulation), SpeciesAttributes.PopulationPressure);
        return Random.value <= survivalChance;
    }

    void OnDeath()
    {
        _population.RemoveOrganism(this);
        Destroy(gameObject);
    }

    void Update()
    {    
        if (_isMovingToMate) MoveToMate();
        else RandomMove();
    }

    void RandomMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, SpeciesAttributes.StartingSpeed * Time.deltaTime);
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

    void LookForAMate()
    {
        switch (SpeciesAttributes.TypeOfMating)
        {
            case TypeOfMating.Random:
                LookForAMateRandom();
                break;

            case TypeOfMating.Closest:
                LookForAMateClosest();
                break;

            case TypeOfMating.Affinity:
                LookForAMateAffinity();
                break;

            default:
                LookForAMateRandom();
                break;
        }
    }

    void LookForAMateRandom()
    {
        _mate = _population.OrganismList[Random.Range(0, _population.OrganismList.Count)];
        _isMovingToMate = true;
    }

    void LookForAMateClosest()
    {
        List<int> possibleMates = new List<int>();

        for (int i = 0; i < _population.OrganismList.Count; i++)
        {
            //bool shouldIgnore = false;
            //foreach (int i in ignore) if (i == j) shouldIgnore = true;

            float distance = Vector3.Distance(transform.position, _population.OrganismList[i].transform.position);
            if (_population.OrganismList[i].ThisIdentity.ID == ThisIdentity.ID) continue;
            else if (distance < SpeciesAttributes.LookForMateViewDistance)
            {
                //Debug.Log("new closest: " + males[j].transform.gameObject.name);
                possibleMates.Add(i);
            }
        }
        if (possibleMates.Count > 0)
        {
            _mate = _population.OrganismList[Random.Range(0, possibleMates.Count)];
            _isMovingToMate = true;
        }
    }
    void LookForAMateAffinity()
    {

    }

    void MoveToMate()
    {
        if (_mate == null) { LookForAMate(); return; }
        Vector3 direction = (_mate.transform.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;

        _speed = Mathf.Lerp(_speed, SpeciesAttributes.MaxSpeed, Time.deltaTime * SpeciesAttributes.IncreaseSpeedRate);

        if (Vector3.Distance(transform.position, _mate.transform.position) < (transform.GetComponent<Renderer>().bounds.size.x / 2)) 
        { 
            CreateChild(); 
            _isMovingToMate = false;
            StartCoroutine(LifeCycle());
            _speed = SpeciesAttributes.StartingSpeed;
        }
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
                childIdentity.ThisGenotype = GenomeManager.Instance.SexualBasicReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype);
                break;

            case TypeOfReproduction.SexualWithRecombination:
                childIdentity.ThisGenotype = GenomeManager.Instance.SexualWithRecombinationReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype);
                break;

            default:
                childIdentity.ThisGenotype = GenomeManager.Instance.SexualBasicReproduction(childIdentity.ThisGenotype, ThisIdentity.ThisGenotype, _mate.ThisIdentity.ThisGenotype);
                break;
        }
        child.GetComponent<Organism>().ThisIdentity = childIdentity;
    }

}


