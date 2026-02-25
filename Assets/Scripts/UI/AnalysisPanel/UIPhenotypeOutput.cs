using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPhenotypeOutput : MonoBehaviour
{
    UIGraph _graph;

    [SerializeField] string _popKey;
    [SerializeField] ColorComponent _colorToPlot;

    private void Start()
    {
        _graph = GetComponent<UIGraph>();
    }

    public void ShowData(List<AnalyzedPhenotype> populations)
    {

        foreach (AnalyzedPhenotype populationMean in populations)
        {
            if (populationMean.PopKey != _popKey) continue;

            float y = 0;
            Color colorToDisplay = new Color(0, 0, 0, 1);
            if (_colorToPlot == ColorComponent.Red) { y = populationMean.PhenotypeNormalizedValue.x; colorToDisplay = new Color(y, 0, 0, 1); }
            if (_colorToPlot == ColorComponent.Green) { y = populationMean.PhenotypeNormalizedValue.y; colorToDisplay = new Color(0, y, 0, 1); }
            if (_colorToPlot == ColorComponent.Blue) { y = populationMean.PhenotypeNormalizedValue.z; colorToDisplay = new Color(0, 0, y, 1); }
            float x = populationMean.Generation;


            bool foundColor = false;
            int i = 0;
            foreach (var color in _graph._colorList)
            {
                if(color == colorToDisplay)
                {
                    _graph.Draw(new Vector2(x, y), i);
                    foundColor = true;
                    break;
                }
                i++;
            }
            if(!foundColor)
            {
                _graph._colorList.Add(colorToDisplay);
                _graph.Draw(new Vector2(x, y), i);

            }


            
        }
        //_graph.DrawMissingDots();
    }
}

public enum ColorComponent
{
    Red,
    Green,
    Blue,
}