using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    public Image Image;
    public TMP_Text Text;
    private void Start()
    {
        StartCoroutine(UpdateColor());
    }

    IEnumerator UpdateColor()
    {
        float numOfOrganisms = 0;
        float red = 0; float green = 0; float blue = 0;
        foreach (Population pop in populationList)
        {
            foreach (Organism o in pop.OrganismList)
            {
                red += o.Phenotype.PhenotypeColor.r;
                green += o.Phenotype.PhenotypeColor.g;
                blue += o.Phenotype.PhenotypeColor.b;
            }
            numOfOrganisms += pop.OrganismList.Count; 
        }

        Image.color = new Color(red / numOfOrganisms, green / numOfOrganisms, blue / numOfOrganisms, 1);
        Text.text = (red / numOfOrganisms).ToString();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(UpdateColor());
    }

    List<Population> populationList = new List<Population>();
    public Population AddOrganism(Organism organisim, string SpeciesKey)
    {
        foreach (Population population in populationList) 
        { 
            if (population.SpeciesKey == SpeciesKey) 
            { 
                population.OrganismList.Add(organisim); 
                return population; 
            } 
        }
        Population newPop = new Population();
        newPop.SpeciesKey = SpeciesKey;
        newPop.PopKey = SpeciesKey + "_POP";
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