using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesAttributes : MonoBehaviour
{
    [Header("Species Config:")]
    public string SpeciesKey;
    public GameObject Prefab;
    public int ChildrenPerMating;
    public int MatingsPerLife;
    public float LookForMateViewDistance;
    public float LifespanInSeconds;
    public TypeOfReproduction TypeOfReproduction;
    public TypeOfMating TypeOfMating;
    public int HaploidNumber;
    public Vector2 DistanceRange;
    public float StartingSpeed;
    public float MaxSpeed;
    public float IncreaseSpeedRate;   
    
    [Header("Population Config:")]
    public string PopulationKey;
    public int MaxPopulation;
    public int InitialPopulation;
    public List<string> PopsToMigrateTo;
    [Range(0, 1)]public List<float> MigrationChancePerIndividual;
    public bool CanReproduceWithOtherPops;
    public Vector2 InitialPosition;
}
public enum TypeOfReproduction
{
    Merge,
    SexualBasic,
    SexualWithRecombination
}

public enum TypeOfMating
{
    Random,
    RandomInRange,
    Closest,
    Affinity,
    Endogamy
}