using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    public List<List<PopulationGen>> PopulationGenList = new List<List<PopulationGen>>(); // each list is a different population, and each populationGen is a generation
    public List<PopulationGen> AddOrganism(Organism organisim, SpeciesAttributes speciesAttributes)
    {
        foreach (List<PopulationGen> population in PopulationGenList) 
        { 
            if (population[0].PopKey == speciesAttributes.PopulationKey) 
            {
                if (population.Count - 1 < organisim.ThisIdentity.Generation) population.Add(new PopulationGen());
                population[organisim.ThisIdentity.Generation].OrganismList.Add(organisim); 
                return population; 
            } 
        }
        List<PopulationGen> newPop = new List<PopulationGen>();
        PopulationGen firstGen = new PopulationGen();
        firstGen.SpeciesKey = speciesAttributes.SpeciesKey;
        firstGen.PopKey = speciesAttributes.PopulationKey;
        firstGen.PopPosition = speciesAttributes.InitialPosition;
        firstGen.OrganismList.Add(organisim);
        firstGen.PopID = PopulationGenList.Count;
        newPop.Add(firstGen);
        PopulationGenList.Add(newPop);
        return newPop;
    }

    public List<PopulationGen> MigrateOrganism(Organism organisim, string popKey)
    {
        foreach (List<PopulationGen> population in PopulationGenList)
        {
            if (population[0].PopKey == popKey)
            {
                if (population.Count - 1 < organisim.ThisIdentity.Generation) population.Add(new PopulationGen());
                population[organisim.ThisIdentity.Generation].OrganismList.Add(organisim);
                return population;
            }
        }
        return null;
    }

}

public class PopulationGen
{
    public void RemoveOrganism(Organism organisim)
    {
        OrganismList.Remove(organisim);
    }
    public int PopID;
    public int Gen;
    public string PopKey;
    public string SpeciesKey;
    public Vector2 PopPosition;
    public List<Organism> OrganismList = new List<Organism>();
}