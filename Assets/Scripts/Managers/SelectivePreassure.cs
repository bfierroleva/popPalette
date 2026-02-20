using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectivePreassure : MonoBehaviour
{
    public float CostOfColor;
    public Color TestColor;
    public bool IsColorSelected;
    public float SelectivePreassureStrenght;
    public float SelectiveCurve;
    public int IsColorRedSelected;  
    [Range(0, 255)] public int SelectedColorR;
    public int isColorGreenSelected;
    [Range(0, 255)] public int SelectedColorG;
    public int isColorBlueSelected;
    [Range(0, 255)] public int SelectedColorB;

    public float CalculateFitness(Color color)
    {
        if (!IsColorSelected) { return 1; }
        //float closeToR = FitnessFunction(Mathf.Abs(((float)SelectedColorR / 255.0f) - color.r), SelectivePreassureStrenght, SelectiveCurve);
        //float closeToG = FitnessFunction(Mathf.Abs(((float)SelectedColorG / 255.0f) - color.g), SelectivePreassureStrenght, SelectiveCurve);
        //float closeToB = FitnessFunction(Mathf.Abs(((float)SelectedColorB / 255.0f) - color.b), SelectivePreassureStrenght, SelectiveCurve);
        //Debug.Log("closeToR: " + closeToR + " = " + " 1 - | " + ((float)SelectedColorR / 255.0f) + " - " + color.r + " |");
        //Debug.Log("closeToG: " + closeToG + " = " + " 1 - | " + ((float)SelectedColorG / 255.0f) + " - " + color.g + " |");
        //Debug.Log("closeToB: " + closeToB + " = " + " 1 - | " + ((float)SelectedColorB / 255.0f) + " - " + color.b + " |");
        float fitness = 0;

        if (IsColorRedSelected == 1) fitness += FitnessFunction(Mathf.Abs(((float)SelectedColorR / 255.0f) - color.r), SelectivePreassureStrenght, SelectiveCurve);
        if (isColorGreenSelected == 1) fitness += FitnessFunction(Mathf.Abs(((float)SelectedColorG / 255.0f) - color.g), SelectivePreassureStrenght, SelectiveCurve);
        if (isColorBlueSelected == 1) fitness += FitnessFunction(Mathf.Abs(((float)SelectedColorB / 255.0f) - color.b), SelectivePreassureStrenght, SelectiveCurve);


        fitness = fitness / (IsColorRedSelected + isColorGreenSelected + isColorBlueSelected);

        //fitness = fitness / Mathf.Clamp((Mathf.Pow((color.r + color.g + color.b + 0.0001f), CostOfColor)),1 , Mathf.Infinity); rehacer
        Debug.Log(fitness);
        return fitness;
    }

    float FitnessFunction(float phenotipicDifferenceNormalized, float selectiveStrenght, float selectiveCurve)
    {
        return 1 - Mathf.Pow(phenotipicDifferenceNormalized, 1 / selectiveCurve) * selectiveStrenght;
    }
}
