using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLocus", menuName = "Locus")]
public class LocusSO : ScriptableObject
{
    public int ChromosomeIndex;
    public string Key;
    public Vector2 AlleleValues;
}