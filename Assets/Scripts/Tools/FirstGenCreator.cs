using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;


public class FirstGenCreator : MonoBehaviour
{
    public List<LocusSO> LocusSOList;
    public List<Vector3> AlleleList = new List<Vector3>(); //x=locusIndex y=relativeFrequenzy z=value
    public List<Locus> LocusList = new List<Locus>();
}


