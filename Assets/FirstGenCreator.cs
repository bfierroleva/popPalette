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
    public List<LocusSO> LocusList;

}
/*
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
            creator.ChromosomePairList = FirstGenCreator.CreateMyScriptableObject();
            creator.ChromosomePairList.Data = new List<List<Locus>>();
        }

        if (creator.ChromosomePairList != null)
        {
            if (creator.ChromosomePairList.Data.Count < creator.HaploidNumber)
                creator.ChromosomePairList.Data.Add(new List<Locus>());

            for (int i = 0; i < creator.ChromosomePairList.Data.Count; i++)
            {
                EditorGUILayout.LabelField("Chromosome Pair " + i);

                if (GUILayout.Button("Add New Gene"))
                {
                    creator.ChromosomePairList.Data[i].Add(new Locus());
                }
                if (GUILayout.Button("Clear Genes"))
                {
                    creator.ChromosomePairList.Data[i] = new List<Locus>();
                }
                for (int j = 0; j < creator.ChromosomePairList.Data[i].Count; j++)
                {
                    EditorGUILayout.LabelField("Gene" + j);
                    EditorGUILayout.LabelField("Gene Name");
                    creator.ChromosomePairList.Data[i][j].Key = EditorGUILayout.TextField(creator.ChromosomePairList.Data[i][j].Key);
                    EditorGUILayout.LabelField("Allele Values");
                    creator.ChromosomePairList.Data[i][j].AlleleValues.x = Mathf.Clamp(EditorGUILayout.FloatField(creator.ChromosomePairList.Data[i][j].AlleleValues.x), -1, 1);
                    creator.ChromosomePairList.Data[i][j].AlleleValues.y = Mathf.Clamp(EditorGUILayout.FloatField(creator.ChromosomePairList.Data[i][j].AlleleValues.y), -1, 1);
                    EditorGUILayout.LabelField("");
                }
            }
        }
    }
}
*/

