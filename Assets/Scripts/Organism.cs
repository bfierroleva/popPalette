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

    private Vector2 _boundsX;
    private Vector2 _boundsY;
    private float _speed;

    public void OnStart()
    {
        FirstGenAttributes = transform.GetComponent<FirstGenCreator>();
        Phenotype = GetComponent<Phenotype>();
        SpeciesAttributes = GetComponent<SpeciesAttributes>();
        Collider2D colliderBOunds = FindObjectOfType<BoundsSprite>().GetComponent<Collider2D>();
        _boundsX = new Vector2(colliderBOunds.bounds.min.x, colliderBOunds.bounds.max.x);
        _boundsY = new Vector2(colliderBOunds.bounds.min.y, colliderBOunds.bounds.max.y);

        _manager = FindObjectOfType<IdentitiesManager>().gameObject;
        _identityManager = _manager.GetComponent<IdentitiesManager>();
        _matingManager = _manager.GetComponent<MatingManager>();

        if (!_isFirstGen) { Destroy(transform.GetComponent<FirstGenCreator>()); }
        else WhenFirstGen();

        OnBirth();
    }

    private void WhenFirstGen()
    {
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
        _population = _manager.GetComponent<PopulationManager>().AddOrganism(this, SpeciesAttributes.SpeciesKey, SpeciesAttributes.PopulationKey);
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
        if (Vector3.Distance(transform.position, _targetPosition) < 0.5f) SetNewRandomDestination();
    }
    void SetNewRandomDestination()
    {
        float distance = Random.Range(SpeciesAttributes.DistanceRange.x, SpeciesAttributes.DistanceRange.y);
        float angle = Random.Range(0, 360);
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        float distanceX = x * distance;
        float distanceY = y * distance;

        
        
        if ((transform.position.x + distanceX) <= _boundsX[0] || (transform.position.x + distanceX) >= _boundsX[1])
        {
            distanceX = distanceX * -1;
        }
        if ((transform.position.y + distanceY) <= _boundsY[0] || (transform.position.y + distanceY) >= _boundsY[1])
        {
            distanceY = distanceY * -1;
        }
        
        _targetPosition = new Vector2(transform.position.x + distanceX, transform.position.y + distanceY);
    }

    void LookForAMate()
    {
        switch (SpeciesAttributes.TypeOfMating)
        {
            case TypeOfMating.Random:
                LookForAMateRandom();
                break;

            case TypeOfMating.RandomInRange:
                LookForAMateRandomInRange();
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
        List<Organism> possibleMates = new List<Organism>();
        possibleMates.AddRange(_population.OrganismList);
        if (SpeciesAttributes.CanReproduceWithOtherPops) 
        {
            foreach (Population pop in FindObjectOfType<PopulationManager>().PopulationList)
            {
                if (pop.SpeciesKey ==  SpeciesAttributes.SpeciesKey) possibleMates.AddRange(pop.OrganismList);
            }
        }
        _mate = possibleMates[Random.Range(0, possibleMates.Count)];
        _isMovingToMate = true;
    }

    void LookForAMateClosest()
    {
        List<Organism> possibleMates = new List<Organism>();
        possibleMates.AddRange(_population.OrganismList);
        if (SpeciesAttributes.CanReproduceWithOtherPops)
        {
            foreach (Population pop in FindObjectOfType<PopulationManager>().PopulationList)
            {
                if (pop.SpeciesKey == SpeciesAttributes.SpeciesKey) possibleMates.AddRange(pop.OrganismList);
            }
        }
        int closest = 0;
        float closestDistance = SpeciesAttributes.LookForMateViewDistance;
        for (int i = 0; i < possibleMates.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, possibleMates[i].transform.position);
            if (possibleMates[i].ThisIdentity.ID == ThisIdentity.ID) continue;
            else if (distance < closestDistance)
            {
                closest = i;
                closestDistance = distance;
            }
        }
        _mate = possibleMates[closest];
        _isMovingToMate = true;
    }

    void LookForAMateRandomInRange()
    {
        List<int> possibleMates = new List<int>();

        for (int i = 0; i < _population.OrganismList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _population.OrganismList[i].transform.position);
            if (_population.OrganismList[i].ThisIdentity.ID == ThisIdentity.ID) continue;
            else if (distance < SpeciesAttributes.LookForMateViewDistance)
            {
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
        Organism childO = child.GetComponent<Organism>();
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
        childO.ThisIdentity = childIdentity;
        childO._isFirstGen = false;
        childO.OnStart();
    }

}


