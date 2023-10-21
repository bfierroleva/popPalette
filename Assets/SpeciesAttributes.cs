using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesAttributes : MonoBehaviour
{
    
    public string SpeciesKey;
    public string SpeciesName;
    [Range(0, 1)] public float MutationChance;
    public int MaxPopulation;
    public float PopulationPressure;
    public GameObject Prefab;
    public int NumberOfMatingsForLifetime;
    public float LookForMateViewDistance;
    public float LifespanInSeconds;
    public TypeOfReproduction TypeOfReproduction;
    public TypeOfMating TypeOfMating;
    public int HaploidNumber;
    public Vector2 DistanceRange;
    public float StartingSpeed;
    public float MaxSpeed;
    public float IncreaseSpeedRate;
    public int InitialPopulation;
    public Vector2 InitialPosition;
    //public List<LocusSO> LocusList;
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
    Closest,
    Affinity
}