using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectivePreassure : MonoBehaviour
{
    public float CostOfColor;
    public Color TestColor;
    public bool IsColorSelected;
    public float SelectivePressure;
    public int IsColorRedSelected;  
    [Range(0, 255)] public int SelectedColorR;
    public int isColorGreenSelected;
    [Range(0, 255)] public int SelectedColorG;
    public int isColorBlueSelected;
    [Range(0, 255)] public int SelectedColorB;

    public float CalculateFitness(Color color)
    {
        float closeToR = Mathf.Pow(1 - Mathf.Abs(((float)SelectedColorR / 255.0f) - color.r), SelectivePressure);
        float closeToG = Mathf.Pow(1 - Mathf.Abs(((float)SelectedColorG / 255.0f) - color.g), SelectivePressure);
        float closeToB = Mathf.Pow(1 - Mathf.Abs(((float)SelectedColorB / 255.0f) - color.b), SelectivePressure);
        //Debug.Log("closeToR: " + closeToR + " = " + " 1 - | " + ((float)SelectedColorR / 255.0f) + " - " + color.r + " |");
        //Debug.Log("closeToG: " + closeToG + " = " + " 1 - | " + ((float)SelectedColorG / 255.0f) + " - " + color.g + " |");
        //Debug.Log("closeToB: " + closeToB + " = " + " 1 - | " + ((float)SelectedColorB / 255.0f) + " - " + color.b + " |");
        float fitness = 1;       
        if (IsColorRedSelected == 1) fitness += closeToR;
        if (isColorGreenSelected == 1) fitness += closeToG;
        if (isColorBlueSelected == 1) fitness += closeToB;
        if (IsColorSelected)
        {
            fitness -= 1;
            fitness = fitness / (IsColorRedSelected + isColorGreenSelected + isColorBlueSelected);
        }
        

        //Debug.Log("color: " + (color.r + color.g + color.b));

        fitness = fitness / Mathf.Clamp((Mathf.Pow((color.r + color.g + color.b + 0.0001f), CostOfColor)),1 , Mathf.Infinity);

        return fitness;
    }
}
