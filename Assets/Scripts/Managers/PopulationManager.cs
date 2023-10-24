using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    public Image Image;
    public TMP_Text AveragePhenotype;
    public TMP_Text PopText;
    public TMP_Text SurvChanceText;
    public TMP_Text HighRed, MedRed, LowRed;
    public float highRedFrequency, medRedFrequency, lowRedFrequency; 

    private void Start()
    {
        //StartCoroutine(UpdateColor());
    }

    IEnumerator UpdateColor()
    {
        float numOfOrganisms = 0;
        float red = 0, green = 0, blue = 0;
        int highReds = 0, medReds = 0, lowReds = 0;
        foreach (Population pop in populationList)
        {
            foreach (Organism o in pop.OrganismList)
            {
                red += o.Phenotype.PhenotypeColor.r;
                //Debug.Log(o.Phenotype.PhenotypeColor.r + " > " + (150 / 255));
                if (o.Phenotype.PhenotypeColor.r > (150.0f / 255.0f)) highReds++;
                else if (o.Phenotype.PhenotypeColor.r < (150.0f / 255.0f) && o.Phenotype.PhenotypeColor.r > (100.0f / 255.0f)) medReds++;
                else if (o.Phenotype.PhenotypeColor.r < (100.0f / 255.0f)) lowReds++;

                green += o.Phenotype.PhenotypeColor.g;
                blue += o.Phenotype.PhenotypeColor.b;
            }
            numOfOrganisms += pop.OrganismList.Count; 
        }

        Image.color = new Color(red / numOfOrganisms, green / numOfOrganisms, blue / numOfOrganisms, 1);
        highRedFrequency = (highReds / numOfOrganisms);
        HighRed.text = highRedFrequency.ToString();
        medRedFrequency = (medReds / numOfOrganisms);
        MedRed.text = medRedFrequency.ToString();
        lowRedFrequency = (lowReds / numOfOrganisms);
        LowRed.text = lowRedFrequency.ToString();

        AveragePhenotype.text = (red / numOfOrganisms).ToString();

        PopText.text = populationList[0].OrganismList.Count.ToString();

        SurvChanceText.text = ((red / numOfOrganisms) * Mathf.Pow(1 - (numOfOrganisms / populationList[0].OrganismList[0].SpeciesAttributes.MaxPopulation), populationList[0].OrganismList[0].SpeciesAttributes.PopulationPressure)).ToString();

        yield return new WaitForSeconds(1f);

        StartCoroutine(UpdateColor());
    }

    public List<Population> populationList = new List<Population>();
    public Population AddOrganism(Organism organisim, string speciesKey, string populationKey)
    {
        foreach (Population population in populationList) 
        { 
            if (population.PopKey == populationKey) 
            { 
                population.OrganismList.Add(organisim); 
                return population; 
            } 
        }
        Population newPop = new Population();
        newPop.SpeciesKey = speciesKey;
        newPop.PopKey = populationKey;
        newPop.OrganismList.Add(organisim);
        populationList.Add(newPop);
        return newPop;
    }   

}

public class Population
{
    public void RemoveOrganism(Organism organisim)
    {
        OrganismList.Remove(organisim);
    }

    public string PopKey;
    public string SpeciesKey;
    public List<Organism> OrganismList = new List<Organism>();
}