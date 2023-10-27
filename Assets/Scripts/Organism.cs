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

    [SerializeField] private float _shrinkingDuration;

    private GameObject _manager;
    private IdentitiesManager _identityManager;
    private GenerationManager _generationManager;

    public Phenotype Phenotype;
    public SpeciesAttributes SpeciesAttributes;
    public Identity ThisIdentity;
    private Population _population;
    private Organism _mate;

    private Vector3 _targetPosition;
    private bool _isMovingToMate = false;
    private int _timesMated;
    private bool _isTryingToDie = false;
    private bool _isDying = false;
    private bool _isGrowing = false;

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
        _generationManager = _manager.GetComponent<GenerationManager>();

        transform.localScale = Vector3.zero;

        if (!_isFirstGen) { Destroy(transform.GetComponent<FirstGenCreator>()); }
        else WhenFirstGen();

        OnBirth();
    }

    private void WhenFirstGen()
    {
        ThisIdentity = _identityManager.CreateNewIdentity();
        ThisIdentity.Generation = 0;
        _generationManager.LifeTime = SpeciesAttributes.LifespanInSeconds;
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
        Phenotype.SetPhenotype(ThisIdentity);
        _population = _manager.GetComponent<PopulationManager>().AddOrganism(this, SpeciesAttributes.SpeciesKey, SpeciesAttributes.PopulationKey);
        if (!_isFirstGen) if (!CanSurviveBirth()) { OnDeath(true); return; }

        _isGrowing = true;

        if ((_generationManager.GenerationPopCount.Count - 1) < ThisIdentity.Generation) _generationManager.GenerationPopCount.Add(0);
        _generationManager.GenerationPopCount[ThisIdentity.Generation]++;

        _speed = SpeciesAttributes.StartingSpeed;
        SetNewRandomDestination();
        if (_isFirstGen) StartLifeCycle();
        _generationManager.newGenerationCycle += StartLifeCycle;       
    }

    void StartLifeCycle()
    {
        StartCoroutine(TryToDie());
        if (_generationManager.CurrentGen == ThisIdentity.Generation) StartCoroutine(MatingCycle()); 
    }

    float randomFractionUntilMating = 0;
    IEnumerator MatingCycle()
    {
        if (_timesMated == 0)
        {
            randomFractionUntilMating = Random.Range(0.2f, 0.5f);
            yield return new WaitForSeconds(SpeciesAttributes.LifespanInSeconds * randomFractionUntilMating);
        }    
        if (_timesMated < SpeciesAttributes.MatingsPerLife)
        {
            LookForAMate();
        }
        
    }

    IEnumerator TryToDie()
    {
        yield return new WaitForSeconds(SpeciesAttributes.LifespanInSeconds - _shrinkingDuration);
        _isTryingToDie = true;
    }

    bool CanSurviveBirth()
    {

        float survivalChance = 0;
        float fitness = _manager.GetComponent<SelectivePreassure>().CalculateFitness(Phenotype.PhenotypeColor);
        //survivalChance = fitness * (1 - (_generationManager.GenerationPopCount[ThisIdentity.Generation - 1] / (float)SpeciesAttributes.MaxPopulation));
        survivalChance = fitness * (1 - _population.OrganismList.Count / (float)SpeciesAttributes.MaxPopulation);
        return Random.value <= survivalChance;
    }

    void OnDeath(bool _isNewBorn)
    {
        if (!_isNewBorn) _isDying = true;
        _generationManager.newGenerationCycle -= StartLifeCycle;
        _population.RemoveOrganism(this);
        Destroy(gameObject, _shrinkingDuration);
    }

    void Update()
    {
        if (_isDying)
        {
            float scaleChange = Time.deltaTime / _shrinkingDuration;
            transform.localScale -= new Vector3(scaleChange, scaleChange, scaleChange);
        }
        if (_isGrowing)
        {
            if (transform.localScale.x < 1)
            {
                float scaleChange = Time.deltaTime / _shrinkingDuration;
                transform.localScale += new Vector3(scaleChange, scaleChange, scaleChange);
            }
            else
            {
                transform.localScale = Vector3.one;
                _isGrowing = false;
            }
            
        }
        if (_isMovingToMate) MoveToMate();
        else RandomMove();
        if (_isTryingToDie && _timesMated >= SpeciesAttributes.MatingsPerLife) OnDeath(false);
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
            _timesMated++;
            for (int i = 0; i < SpeciesAttributes.ChildrenPerMating; i++)
            {
                CreateChild();
            }        
                     
            _isMovingToMate = false;
            StartCoroutine(MatingCycle());
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
        childIdentity.Generation = ThisIdentity.Generation + 1;
        childIdentity.ThisGenotype = new Genotype();
        childIdentity.ThisGenotype.HaploidNumber = ThisIdentity.ThisGenotype.HaploidNumber;
        childIdentity.ThisGenotype.Karyotype = new List<ChromosomePair>();
        for (int i = 0; i < ThisIdentity.ThisGenotype.Karyotype.Count; i++)
        {
            ChromosomePair newPair = new ChromosomePair();
            newPair.Index = ThisIdentity.ThisGenotype.Karyotype[i].Index;

            childIdentity.ThisGenotype.Karyotype.Add(newPair);   
            
            for (int j = 0; j < ThisIdentity.ThisGenotype.Karyotype[i].LocusList.Count; j++)
            {
                Locus newLocus = new Locus();
                Locus p1Locus = ThisIdentity.ThisGenotype.Karyotype[i].LocusList[j];
                newLocus.Key = p1Locus.Key;
                newLocus.IsFunctional = p1Locus.IsFunctional;
                newLocus.MutationChance = p1Locus.MutationChance;
                newLocus.MutationMaxRange = p1Locus.MutationMaxRange;
                newLocus.ChromosomeIndex = p1Locus.ChromosomeIndex;
                newLocus.AlleleValuesRange.x = p1Locus.AlleleValuesRange.x;
                newLocus.AlleleValuesRange.y = p1Locus.AlleleValuesRange.y;

                childIdentity.ThisGenotype.Karyotype[i].LocusList.Add(newLocus);
            }
        }

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
        //Debug.Log(gameObject.name + "   " + ThisIdentity.ThisGenotype.Karyotype[0].LocusList[0].AlleleValues + "  y  " + _mate.gameObject.name + "  " + _mate.ThisIdentity.ThisGenotype.Karyotype[0].LocusList[0].AlleleValues + "  =  " + childO.ThisIdentity.ThisGenotype.Karyotype[0].LocusList[0].AlleleValues);
        
        childO._isFirstGen = false;
        childO.OnStart();
    }

}


