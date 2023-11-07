using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGenotypeFreqOutput : MonoBehaviour
{
    UIGraph _graph;

    [SerializeField] string _popKey;
    [SerializeField] List<Vector2Int> _genotypeToColorIndex;

    private void Start()
    {
        _graph = GetComponent<UIGraph>();
    }
    public void ShowData(List<AnalyzedGenotype> genotypes)
    {
        foreach (AnalyzedGenotype genotype in genotypes)
        {
            if (genotype.PopKey != _popKey) continue;

            int i = 0;
            int colorIndex = 0;
            foreach (Vector2Int genotypeToColor in _genotypeToColorIndex)
            {
                if (genotypeToColor == genotype.AlleleValues) colorIndex = i;
                i++;
            }

            float y = genotype.FrequencyRel;
            float x = genotype.Generation;

            _graph.Draw(new Vector2(x, y), colorIndex);
        }
        _graph.DrawMissingDots();
    }
}
