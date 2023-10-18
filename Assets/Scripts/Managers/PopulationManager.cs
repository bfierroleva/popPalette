using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    List<Population> populations = new List<Population>();
    public Population AddOrganism(Organism organisim, string SpeciesKey)
    {
        foreach (Population population in populations) 
        { 
            if (population.SpeciesKey == SpeciesKey) 
            { 
                population.PopulationList.Add(organisim); 
                return population; 
            } 
        }
        Population newPop = new Population();
        newPop.SpeciesKey = SpeciesKey;
        newPop.PopKey = SpeciesKey + "_POP";
        newPop.PopulationList.Add(organisim);
        populations.Add(newPop);
        return newPop;
    }
}

public class Population
{
    public string PopKey;
    public string SpeciesKey;
    public List<Organism> PopulationList = new List<Organism>();
}