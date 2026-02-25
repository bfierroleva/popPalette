using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Linq;

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
    private List<PopulationGen> _population = new List<PopulationGen>();
    private Organism _mate;

    private Vector3 _targetPosition;
    private bool _isMovingToMate = false;
    private int _timesMated;
    private bool _isTryingToDie = false;
    private bool _isDying = false;
    private bool _isGrowing = false;
    public bool IsMigrating = false;
    private List<PopulationGen> _popToMigrateTo = new List<PopulationGen>();

    public Collider2D Bounds;
    public Vector2 BoundsX;
    public Vector2 BoundsY;
    private float _speed;

    public void OnStart()
    {
        FirstGenAttributes = transform.GetComponent<FirstGenCreator>();
        Phenotype = GetComponent<Phenotype>();
        SpeciesAttributes = GetComponent<SpeciesAttributes>();
        

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
        _population = _manager.GetComponent<PopulationManager>().AddOrganism(this, SpeciesAttributes);
        if (!_isFirstGen)
        {
            if (!CanSurviveBirth()) { OnDeath(true); return; }

            _popToMigrateTo = _population;
            if (SpeciesAttributes.PopsToMigrateTo.Count > 0) TryToMigrate();
        }
        else
        {
            _popToMigrateTo = _population;
        }

        AnalysisManager analysisManager = _manager.GetComponent<AnalysisManager>();

        foreach (ChromosomePair chromosomePair in ThisIdentity.ThisGenotype.Karyotype)
        {
            foreach (Locus locus in chromosomePair.LocusList)
            {
                analysisManager.AnalyzeGenotype(locus.AlleleValues,locus.Key, _popToMigrateTo[0].PopKey, ThisIdentity.Generation);
            }
        }

        

        _isGrowing = true;

        if ((_generationManager.GenerationPopCount.Count - 1) < ThisIdentity.Generation) _generationManager.GenerationPopCount.Add(0);
        _generationManager.GenerationPopCount[ThisIdentity.Generation]++;

        _speed = SpeciesAttributes.StartingSpeed;
        SetNewRandomDestination();
        if (_isFirstGen) StartLifeCycle();
        
        _generationManager.NewGenerationCycle += StartLifeCycle;       
    }

    void TryToMigrate()
    {
        float randomValue = UnityEngine.Random.value;
        int newPop = 0;
        float accumulatedChance = 0;
        bool willMigrate = false;

        if (SpeciesAttributes.MigrationChancePerIndividual.Sum() > 1) { Debug.LogError("Sum of probabilities greater than 1"); return; }
        if (SpeciesAttributes.MigrationChancePerIndividual.Sum() > 1) { Debug.LogError("Return not working"); }

        foreach (float chance in SpeciesAttributes.MigrationChancePerIndividual)
        {
            
            accumulatedChance += chance;
            if (randomValue <= accumulatedChance) { willMigrate = true; break; }
            newPop++;
        }

        if (willMigrate)
        {
           
            foreach (List<PopulationGen> pop in _manager.GetComponent<PopulationManager>().PopulationGenList)
            {
                if (pop[0].PopKey == SpeciesAttributes.PopsToMigrateTo[newPop]) _popToMigrateTo = pop;
                IsMigrating = true;
            }
            
        }
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
            randomFractionUntilMating = UnityEngine.Random.Range(0.2f, 0.5f);
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
        float fitness = _manager.GetComponent<SelectivePreassure>().CalculateFitness(Phenotype.PhenotypeColor, SpeciesAttributes.PopulationKey);
        //survivalChance = fitness * (1 - (_generationManager.GenerationPopCount[ThisIdentity.Generation - 1] / (float)SpeciesAttributes.MaxPopulation));
        survivalChance = fitness * (1 - _population[ThisIdentity.Generation].OrganismList.Count / (float)SpeciesAttributes.MaxPopulation);
        return UnityEngine.Random.value <= survivalChance;
    }

    void OnDeath(bool _isNewBorn)
    {
        if (!_isNewBorn) _isDying = true;
        _generationManager.NewGenerationCycle -= StartLifeCycle;
        _population[ThisIdentity.Generation].RemoveOrganism(this);
        Destroy(gameObject, _shrinkingDuration);
    }

    void Update()
    {
        if (_isDying) Shrink();
        if (_isGrowing) Grow();
        if (IsMigrating) Migrate();
        if (_isMovingToMate) MoveToMate();
        else RandomMove();
        if (_isTryingToDie && _timesMated >= SpeciesAttributes.MatingsPerLife) OnDeath(false);
    }

    void Migrate()
    {
        transform.position = Vector3.MoveTowards(transform.position, _popToMigrateTo[0].PopPosition, SpeciesAttributes.MaxSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _popToMigrateTo[0].PopPosition) < 0.5f)
        {
            _population[ThisIdentity.Generation].RemoveOrganism(this);
            _population = _manager.GetComponent<PopulationManager>().MigrateOrganism(this, _popToMigrateTo[0].PopKey);
            IsMigrating = false;

            int i = 0;
            foreach (GameObject populationPrefab in _manager.GetComponent<FirstGenManager>().PopulationPrefabList)
            {
                if (_popToMigrateTo[0].PopKey == populationPrefab.GetComponent<SpeciesAttributes>().PopulationKey)
                {

                    SpeciesAttributes.Prefab = populationPrefab;

                    BoundsX = new Vector2(_manager.GetComponent<FirstGenManager>().PopulationBounds[i].bounds.min.x, _manager.GetComponent<FirstGenManager>().PopulationBounds[i].bounds.max.x);
                    BoundsY = new Vector2(_manager.GetComponent<FirstGenManager>().PopulationBounds[i].bounds.min.y, _manager.GetComponent<FirstGenManager>().PopulationBounds[i].bounds.max.y);
                    Bounds  = _manager.GetComponent<FirstGenManager>().PopulationBounds[i];
                }
                else
                {
                    i++;
                }
            }
            SetNewRandomDestination();
        }
        
    }

    void Shrink()
    {
        float scaleChange = Time.deltaTime / _shrinkingDuration;
        transform.localScale -= new Vector3(scaleChange, scaleChange, scaleChange);
    }

    void Grow()
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

    void RandomMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, SpeciesAttributes.StartingSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.5f) SetNewRandomDestination();
    }
    void SetNewRandomDestination()
    {
        float distance = UnityEngine.Random.Range(SpeciesAttributes.DistanceRange.x, SpeciesAttributes.DistanceRange.y);
        float angle = UnityEngine.Random.Range(0, 360);
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        float distanceX = x * distance;
        float distanceY = y * distance;

        Bounds bounds = Bounds.bounds;

        float targetX = Mathf.Clamp(transform.position.x + distanceX, bounds.min.x, bounds.max.x);
        float targetY = Mathf.Clamp(transform.position.y + distanceY, bounds.min.y, bounds.max.y);

        _targetPosition = new Vector2(targetX, targetY);

        /*if ((transform.position.x + distanceX) <= BoundsX[0] || (transform.position.x + distanceX) >= BoundsX[1])
        {
            distanceX = distanceX * -1;
        }
        if ((transform.position.y + distanceY) <= BoundsY[0] || (transform.position.y + distanceY) >= BoundsY[1])
        {
            distanceY = distanceY * -1;
        }
        
        _targetPosition = new Vector2(transform.position.x + distanceX, transform.position.y + distanceY);
        */
        
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

            case TypeOfMating.Endogamy:
                LookForAMateEndogamy();
                break;

            default:
                LookForAMateRandom();
                break;
        }
    }

    void LookForAMateRandom()
    {
        List<Organism> possibleMates = new List<Organism>();
        possibleMates.AddRange(_population[ThisIdentity.Generation].OrganismList);
        if (SpeciesAttributes.CanReproduceWithOtherPops) 
        {
            foreach (List<PopulationGen> pop in FindObjectOfType<PopulationManager>().PopulationGenList)
            {
                if (pop[0].SpeciesKey ==  SpeciesAttributes.SpeciesKey) possibleMates.AddRange(pop[ThisIdentity.Generation].OrganismList);
            }
        }
        _mate = possibleMates[UnityEngine.Random.Range(0, possibleMates.Count)];
        while (_mate.IsMigrating)
        {
            _mate = possibleMates[UnityEngine.Random.Range(0, possibleMates.Count)];
        }
        _isMovingToMate = true;
    }

    void LookForAMateClosest()
    {
        List<Organism> possibleMates = new List<Organism>();
        possibleMates.AddRange(_population[ThisIdentity.Generation].OrganismList);
        if (SpeciesAttributes.CanReproduceWithOtherPops)
        {
            foreach (List<PopulationGen> pop in FindObjectOfType<PopulationManager>().PopulationGenList)
            {
                if (pop[0].SpeciesKey == SpeciesAttributes.SpeciesKey) possibleMates.AddRange(pop[ThisIdentity.Generation].OrganismList);
            }
        }
        int closest = 0;
        float closestDistance = SpeciesAttributes.LookForMateViewDistance;
        for (int i = 0; i < possibleMates.Count; i++)
        {
            if (possibleMates[i].ThisIdentity.ID == ThisIdentity.ID) continue;
            if (possibleMates[i].IsMigrating) continue;
            float distance = Vector3.Distance(transform.position, possibleMates[i].transform.position);
            
            if (distance < closestDistance)
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

        for (int i = 0; i < _population[ThisIdentity.Generation].OrganismList.Count; i++)
        {
            if (_population[ThisIdentity.Generation].OrganismList[i].ThisIdentity.ID == ThisIdentity.ID) continue;
            if (_population[ThisIdentity.Generation].OrganismList[i].IsMigrating) continue;
            float distance = Vector3.Distance(transform.position, _population[ThisIdentity.Generation].OrganismList[i].transform.position);
            if (distance < SpeciesAttributes.LookForMateViewDistance)
            {
                possibleMates.Add(i);
            }
        }
        if (possibleMates.Count > 0)
        {
            _mate = _population[ThisIdentity.Generation].OrganismList[UnityEngine.Random.Range(0, possibleMates.Count)];
            while (_mate.IsMigrating)
            {
                _mate = _population[ThisIdentity.Generation].OrganismList[UnityEngine.Random.Range(0, possibleMates.Count)];
            }
            _isMovingToMate = true;
        }
    }
    void LookForAMateAffinity()
    {

    }

    
    void LookForAMateEndogamy() // MUY INEFICIENTE: rehacer
    {
        /*
        List<Organism> possibleMates = new List<Organism>();
        possibleMates.AddRange(_population.OrganismList);
        if (SpeciesAttributes.CanReproduceWithOtherPops)
        {
            foreach (Population pop in FindObjectOfType<PopulationManager>().PopulationList)
            {
                if (pop.SpeciesKey == SpeciesAttributes.SpeciesKey) possibleMates.AddRange(pop.OrganismList);
            }
        }

        if (_isFirstGen)
        {
            _mate = possibleMates[UnityEngine.Random.Range(0, possibleMates.Count)];
            _isMovingToMate = true;
            return;
        }

        int mostRelated = -1;
        float maxRelatedness = 0;
        for (int i = 0; i < possibleMates.Count; i++)
        {
            if (possibleMates[i].ThisIdentity.ID == ThisIdentity.ID) continue;
            if (possibleMates[i].IsMigrating) continue;
            int generationsApart = 100;

            List<int> parentIDs = new List<int> { ThisIdentity.Parent1ID, ThisIdentity.Parent2ID };
            List<int> parentIDsMate = new List<int> { possibleMates[i].ThisIdentity.Parent1ID, possibleMates[i].ThisIdentity.Parent2ID };

            foreach (int parentID in parentIDs)
            {
                foreach (var parentIDmate in parentIDsMate)
                {
                    if (parentIDmate == parentID) generationsApart = 1;
                }
            }

            /*
            if (generationsApart == 100)
            {
                List<int> grandparentIDs = new List<int>();
                List<int> grandparentIDsMate = new List<int>();

                foreach (Identity identity in _identityManager.AllIdentities)
                {
                    foreach(int parentID in parentIDs)
                    {
                        if (parentID == identity.ID) { grandparentIDs.Add(identity.ID); continue; }
                    }
                    foreach (int grandparentID in parentIDsMate)
                    {
                        if (grandparentID == identity.ID) { grandparentIDsMate.Add(identity.ID); continue; }
                    }
                }

                foreach (int grandparentID in parentIDs)
                {
                    foreach (var grandparentIDmate in parentIDsMate)
                    {
                        if (grandparentIDmate == grandparentID) generationsApart = 2;
                    }
                }
            }
            
            Debug.Log(generationsApart);
            float Relatedness = (1/2) * (float)generationsApart;
            
            if (Relatedness < maxRelatedness)
            {
                mostRelated = i;
                maxRelatedness = Relatedness;
                if (generationsApart == 1) break;
            }
        }
        if(mostRelated != -1)
        {
            _mate = possibleMates[mostRelated];
            _isMovingToMate = true;
        }
        
    */
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
        //Debug.Log(ThisIdentity.ID + " : nuevo hijo con " + _mate.ThisIdentity.ID);
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


        childO.BoundsX = BoundsX;
        childO.BoundsY = BoundsY;
        childO.Bounds = Bounds;
        childO._isFirstGen = false;
        childO.OnStart();
    }

}


