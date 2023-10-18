using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSpecies", menuName = "Species")]
public class SpeciesSO : ScriptableObject
{
    public string SpeciesKey;
    public string SpeciesName;
    [Range(0, 1)] public float MutationChance;
    public int MaxPopulation;
    public GameObject Prefab;
    public int ReprductionCycleInSeconds;
    public float LookForMateViewDistance;
    public int LifespanInSeconds;
    public TypeOfReproduction TypeOfReproduction;
    public int HaploidNumber;
    public Vector2 DistanceRange;
    public float Speed;
    public List<LocusSO> LocusList;
}
