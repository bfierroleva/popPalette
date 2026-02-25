using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIGraph : MonoBehaviour
{
    [SerializeField] private float _yMaxValue;
    [SerializeField] private float _xMaxValue;
    [SerializeField] private RectTransform _graphContainer;
    [SerializeField] private Sprite _circleSprite;
    public List<Color> _colorList;

    private List<List<Vector3>> _dotList = new List<List<Vector3>>();//each list is a genertion, x = x value, y = y value, z = color index

    public void Draw(Vector2 values, int colorIndex)
    {
        if (values.x > _xMaxValue) return;
        float xPos = 1 + (values.x / _xMaxValue) * _graphContainer.sizeDelta.x;
        float yPos = (values.y / _yMaxValue) * _graphContainer.sizeDelta.y;

        CreateDot(new Vector2(xPos, yPos), colorIndex, (int)values.x);
    }

 

    public void DrawMissingDots()
    {
        int currentGen = FindObjectOfType<GenerationManager>().CurrentGen;
        if (currentGen > _xMaxValue) return;

        for (int i = 0; i < _colorList.Count; i++)
        {
            bool hasFoundColor = false;
            foreach (Vector3 dotInfo in _dotList[currentGen])
            {
                if (dotInfo.z == i) { hasFoundColor = true; break; }
            }
            if(!hasFoundColor) 
            {
                float xPos = 1 + (currentGen / _xMaxValue) * _graphContainer.sizeDelta.x;
                float yPos = (0 / _yMaxValue) * _graphContainer.sizeDelta.y;

                CreateDot(new Vector2(xPos, yPos), i, currentGen);
            }
        }
    }

    void CreateDot(Vector2 achoredPosition, int colorIndex, int gen)
    {
        GameObject dot = new GameObject("dot", typeof(Image));
        dot.transform.SetParent(_graphContainer, false);
        dot.GetComponent<Image>().sprite = _circleSprite;
        dot.GetComponent<Image>().color = _colorList[colorIndex];
        RectTransform rectTransform = dot.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = achoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;

        if (_dotList.Count <= gen) _dotList.Add(new List<Vector3>());
        _dotList[gen].Add(new Vector3(achoredPosition.x, achoredPosition.y, colorIndex));

        if (achoredPosition.x <= 1) return;
        bool foundPreviousPoint = false;
        foreach (Vector3 point in _dotList[gen - 1])
        {
            if (point.z == colorIndex) { CreateDotConnection(new Vector2(point.x, point.y), achoredPosition, colorIndex); foundPreviousPoint = true; }
        }
        /*
        if (!foundPreviousPoint)
        {
            Vector2 previousDot = new Vector2(1 + (gen - 1 / _xMaxValue) * _graphContainer.sizeDelta.x, (0 / _yMaxValue) * _graphContainer.sizeDelta.y);
            CreateDot(previousDot, colorIndex, (int)(gen - 1));
            CreateDotConnection(previousDot, achoredPosition, colorIndex);
        }
        */
    }

    void CreateDotConnection(Vector2 dotA, Vector2 dotB, int colorIndex)
    {
        GameObject line = new GameObject("dotConnection", typeof(Image));
        line.transform.SetParent(_graphContainer, false);
        line.GetComponent<Image>().color = new Color(_colorList[colorIndex].r, _colorList[colorIndex].g, _colorList[colorIndex].b, 0.5f);
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 dir = (dotB - dotA).normalized;
        float distance = Vector2.Distance(dotA, dotB);
        rectTransform.sizeDelta = new Vector2(distance, 2.5f);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = dotA + dir * distance * 0.5f;

        float angleInRadians = Mathf.Atan2(dir.y, dir.x);
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angleInDegrees);
    }
}
