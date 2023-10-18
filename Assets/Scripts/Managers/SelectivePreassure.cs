using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectivePreassure : MonoBehaviour
{
    public float CostOfColor;
    public Color TestColor;
    public bool isColorSelected;
    public int isColorRedSelected;
    [Range(0, 255)] public int SelectedColorR;
    public int isColorGreenSelected;
    [Range(0, 255)] public int SelectedColorG;
    public int isColorBlueSelected;
    [Range(0, 255)] public int SelectedColorB;

    public float CalculateFitness(Color color)
    {
        float closeToR = (1 - Mathf.Abs((SelectedColorR / 255) - color.r));
        float closeToG = (1 - Mathf.Abs((SelectedColorR / 255) - color.g));
        float closeToB = (1 - Mathf.Abs((SelectedColorR / 255) - color.b));

        float fitnessBeforeCost = 1;       
        if (isColorRedSelected == 1) fitnessBeforeCost += closeToR;
        if (isColorGreenSelected == 1) fitnessBeforeCost += closeToG;
        if (isColorBlueSelected == 1) fitnessBeforeCost += closeToB;
        if (isColorSelected)
        {
            fitnessBeforeCost -= 1;
            fitnessBeforeCost = fitnessBeforeCost / (isColorRedSelected + isColorGreenSelected + isColorBlueSelected);
        }
        

        //Debug.Log("fitnessBeforeCost: " + fitnessBeforeCost);
        //Debug.Log("color: " + (color.r + color.g + color.b));

        float fitness = fitnessBeforeCost / Mathf.Clamp((Mathf.Pow((color.r + color.g + color.b + 0.0001f), CostOfColor)),1 , Mathf.Infinity);

        return fitness;
    }
}
