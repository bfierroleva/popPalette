using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
[CreateAssetMenu(fileName = "FirstGen", menuName = "FirstGen")]
public class FirstGenCreatorSO : ScriptableObject
{
    [Header("SpeciesConfig")]
    public string SpeciesName;
    public TypeOfReproduction TypeOfReproduction;
    public int HaploidNumber;   

    [Header("OrganismConfig")]
    public NestedGeneList ChromosomePairList;
}

[CustomEditor(typeof(FirstGenCreatorSO))]
public class NestedListEditor : Editor
{
    bool _draw = true;
    public override void OnInspectorGUI()
    {
        FirstGenCreatorSO SO = (FirstGenCreatorSO)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Hide All")) _draw = !_draw;

        if (!_draw) return; 

        if (SO.ChromosomePairList.data.Count < SO.HaploidNumber) SO.ChromosomePairList.data.Add(new List<List<Gene>>());

        if (SO.ChromosomePairList != null)
        {
            if (GUILayout.Button("Reload"))
            {
                SO.ChromosomePairList.data = new List<List<List<Gene>>>();
            }
            for (int i = 0; i < SO.ChromosomePairList.data.Count ; i++)
            {
                EditorGUILayout.LabelField("Chromosome Pair " + i);
                if (SO.ChromosomePairList.data[i].Count < 2) SO.ChromosomePairList.data[i].Add(new List<Gene>());

                if (GUILayout.Button("Add New Gene"))
                {
                    SO.ChromosomePairList.data[i][0].Add(new Gene());
                    SO.ChromosomePairList.data[i][1].Add(new Gene());
                }
                if (GUILayout.Button("Clear Genes"))
                {
                    SO.ChromosomePairList.data[i][0] = new List<Gene>();
                    SO.ChromosomePairList.data[i][1] = new List<Gene>();
                }
                for (int k = 0; k < SO.ChromosomePairList.data[i][0].Count; k++)
                {
                    EditorGUILayout.LabelField("Gene" + k);
                    EditorGUILayout.LabelField("Gene Name");
                    SO.ChromosomePairList.data[i][0][k].Name = EditorGUILayout.TextField(SO.ChromosomePairList.data[i][0][k].Name);
                    SO.ChromosomePairList.data[i][1][k].Name = SO.ChromosomePairList.data[i][0][k].Name;
                    EditorGUILayout.LabelField("Aleles Values");
                    SO.ChromosomePairList.data[i][0][k].AleleValue = Mathf.Clamp(EditorGUILayout.FloatField(SO.ChromosomePairList.data[i][0][k].AleleValue), -1, 1);
                    SO.ChromosomePairList.data[i][1][k].AleleValue = Mathf.Clamp(EditorGUILayout.FloatField(SO.ChromosomePairList.data[i][1][k].AleleValue), -1, 1);
                    EditorGUILayout.LabelField("");
                }
            }
        }
    }
}



[System.Serializable]
public class NestedGeneList
{
    public List<List<List<Gene>>> data = new List<List<List<Gene>>>();
}


public class NestedListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FirstGenCreatorSO SO = (FirstGenCreatorSO)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Add New Chromosome Pair"))
        {
            if (SO.ChromosomePairList.data.Count < SO.HaploidNumber) SO.ChromosomePairList.data.Add(new List<List<Gene>>());
        }

        if (SO.ChromosomePairList != null)
        {
            if (GUILayout.Button("Clear All"))
            {
                SO.ChromosomePairList.data = new List<List<List<Gene>>>();
            }
            for (int i = 0; i < SO.ChromosomePairList.data.Count ; i++)
            {
                EditorGUILayout.LabelField("Chromosome Pair " + i);
                if (GUILayout.Button("Add New Chromosome"))
                {
                    if (SO.ChromosomePairList.data[i].Count < 2) SO.ChromosomePairList.data[i].Add(new List<Gene>());
                }
                if (GUILayout.Button("Clear Chromosomes"))
                {
                    SO.ChromosomePairList.data[i] = new List<List<Gene>>(); 
                }
                for (int j = 0; j < SO.ChromosomePairList.data[i].Count; j++)
                {
                    EditorGUILayout.LabelField(" Chromosome " + j);
                    if (GUILayout.Button("Add New Gene"))
                    {
                        SO.ChromosomePairList.data[i][j].Add(new Gene());
                    }
                    if (GUILayout.Button("Clear Genes"))
                    {
                        SO.ChromosomePairList.data[i][j] = new List<Gene>();
                    }
                    for (int k = 0; k < SO.ChromosomePairList.data[i][j].Count; k++)
                    {
                        SO.ChromosomePairList.data[i][j][k].Name = EditorGUILayout.TextField(SO.ChromosomePairList.data[i][j][k].Name);
                        SO.ChromosomePairList.data[i][j][k].AleleValue = Mathf.Clamp(EditorGUILayout.FloatField(SO.ChromosomePairList.data[i][j][k].AleleValue), -1, 1);
                    }
                }
            }
        }
    }
}
*/


