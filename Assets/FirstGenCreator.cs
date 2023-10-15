using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class FirstGenCreator : MonoBehaviour
{
    [Header("SpeciesConfig")]
    public string SpeciesName;
    public TypeOfReproduction TypeOfReproduction;
    public int HaploidNumber;

    [Header("OrganismConfig")]
    public List<List<List<Gene>>> ChromosomePairList;
    public FirstGenCreator CopyListForm;
}

[CustomEditor(typeof(FirstGenCreator))]
public class NestedListEditor : Editor
{
    bool _draw = true;
    
    public override void OnInspectorGUI()
    {
        FirstGenCreator creator = (FirstGenCreator)target;
        DrawDefaultInspector();      

        if (GUILayout.Button("Hide All")) _draw = !_draw;

        if (!_draw) return;

        if (GUILayout.Button("Reload"))
        {
            if (!creator.CopyListForm) creator.ChromosomePairList = new List<List<List<Gene>>>();
            else { creator.ChromosomePairList = new List<List<List<Gene>>>(); creator.ChromosomePairList.AddRange(creator.CopyListForm.ChromosomePairList); }
        }

        if (creator.ChromosomePairList != null)
        {
            if (creator.ChromosomePairList.Count < creator.HaploidNumber) creator.ChromosomePairList.Add(new List<List<Gene>>());
            
            for (int i = 0; i < creator.ChromosomePairList.Count; i++)
            {
                EditorGUILayout.LabelField("Chromosome Pair " + i);
                if (creator.ChromosomePairList[i].Count < 2) creator.ChromosomePairList[i].Add(new List<Gene>());

                if (GUILayout.Button("Add New Gene"))
                {
                    creator.ChromosomePairList[i][0].Add(new Gene());
                    creator.ChromosomePairList[i][1].Add(new Gene());
                }
                if (GUILayout.Button("Clear Genes"))
                {
                    creator.ChromosomePairList[i][0] = new List<Gene>();
                    creator.ChromosomePairList[i][1] = new List<Gene>();
                }
                for (int k = 0; k < creator.ChromosomePairList[i][0].Count; k++)
                {
                    EditorGUILayout.LabelField("Gene" + k);
                    EditorGUILayout.LabelField("Gene Name");
                    creator.ChromosomePairList[i][0][k].Name = EditorGUILayout.TextField(creator.ChromosomePairList[i][0][k].Name);
                    creator.ChromosomePairList[i][1][k].Name = creator.ChromosomePairList[i][0][k].Name;
                    EditorGUILayout.LabelField("Aleles Values");
                    creator.ChromosomePairList[i][0][k].AleleValue = Mathf.Clamp(EditorGUILayout.FloatField(creator.ChromosomePairList[i][0][k].AleleValue), -1, 1);
                    creator.ChromosomePairList[i][1][k].AleleValue = Mathf.Clamp(EditorGUILayout.FloatField(creator.ChromosomePairList[i][1][k].AleleValue), -1, 1);
                    EditorGUILayout.LabelField("");
                }
            }
        }
    }
}


/*
[System.Serializable]
public class NestedGeneList
{
    public List<List<List<Gene>>> data;
}
*/