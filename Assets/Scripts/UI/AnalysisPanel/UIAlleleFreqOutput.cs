using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAlleleFreqOutput : MonoBehaviour
{
    UIGraph _graph;

    [SerializeField] string _popKey;
    [SerializeField] List<int> _alleleToColorIndex;

    private void Start()
    {
        _graph = GetComponent<UIGraph>();
    }
    public void ShowData(List<AnalyzedAllele> alleles)
    {
        foreach (AnalyzedAllele allele in alleles)
        {
            if (allele.PopKey != _popKey) continue;

            int i = 0;
            int colorIndex = 0;
            foreach (int alleleToColor in _alleleToColorIndex)
            {
                if (alleleToColor == allele.AlleleValue) colorIndex = i;
                i++;
            }

            float y = allele.FrequencyRel;
            float x = allele.Generation;

            _graph.Draw(new Vector2(x, y), colorIndex);
        }
        _graph.DrawMissingDots();
    }
}
