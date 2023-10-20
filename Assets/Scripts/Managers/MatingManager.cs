using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MatingManager : MonoBehaviour
{
    /*
    [SerializeField] List<SpeciesSO> _speciesList;

    private void Start()
    {
        int i = 0;
        foreach (SpeciesSO species in _speciesList) 
        {
            //StartCoroutine(ReproductionCycle(i));
            i++;
        }
    }

    IEnumerator ReproductionCycle(int speciesIndex)
    {
        yield return new WaitForSeconds(_speciesList[speciesIndex].ReprductionCycleInSeconds);
        PairMates(speciesIndex);
        StartCoroutine(ReproductionCycle(speciesIndex));
    }

    void PairMates(int speciesIndex)
    {
        List<Vector2> pairs = new List<Vector2>();
        List<Organism> males = new List<Organism>();
        List<Organism> females = new List<Organism>();
        List<bool> matedMales = new List<bool>();
        List<bool> matedFemales = new List<bool>();
        Organism[] allOrganisims = FindObjectsOfType<Organism>();
        foreach (Organism o in allOrganisims)
        {
            if (o.SpeciesAttributes.SpeciesKey == _speciesList[speciesIndex].SpeciesKey) 
            {
                if (o.IsFemale) { females.Add(o); matedFemales.Add(false); }
                else { males.Add(o); matedMales.Add(false); }
            }
        }
        for (int i = 0; i < females.Count; i++)
        {
            Vector2 newPair = LocateMate(speciesIndex, males, matedMales, i, new List<int>(), females, matedFemales); 
            if (newPair.x != -1 &&  newPair.y != -1) pairs.Add(newPair);
        }
        Reproduce(pairs, speciesIndex, females, males);
        //Debug.LogError(pairs);
    }

    Vector2 LocateMate(int speciesIndex, List<Organism> males, List<bool> matedMales, int indexOfFemaleLooking, List<int> ignore, List<Organism> females, List<bool> matedFemales)
    {
        int closestPossibleMale = -1;
        float closestDistance1 = _speciesList[speciesIndex].LookForMateViewDistance;
        for (int j = 0; j < males.Count; j++)
        {
            bool shouldIgnore = false;
            //Debug.Log(indexOfOrganismLooking + " ignora a " + ignore[0]);

            if (matedMales[j]) shouldIgnore = true;
            foreach (int i in ignore) if (i == j) shouldIgnore = true;
            if (!shouldIgnore)
            {
                float distance = Vector3.Distance(females[indexOfFemaleLooking].transform.position, males[j].transform.position);

                if (distance < closestDistance1)
                {
                    //Debug.Log("new closest: " + males[j].transform.gameObject.name);
                    closestDistance1 = distance;
                    closestPossibleMale = j;
                }
            }
        }

        //if (closestPossibleMale != -1) Debug.Log(females[indexOfFemaleLooking].transform.gameObject.name + " fancies " + males[closestPossibleMale].transform.gameObject.name);

        if (closestPossibleMale == -1) { matedFemales[indexOfFemaleLooking] = true; return new Vector2(-1, -1); }

        //Debug.Log(females[indexOfFemaleLooking].transform.gameObject.name + " mated with " + males[closestPossibleMale].transform.gameObject.name);
        matedFemales[indexOfFemaleLooking] = true;
        matedMales[closestPossibleMale] = true;
        return new Vector2(indexOfFemaleLooking, closestPossibleMale);
    }

    void Reproduce(List<Vector2> pairs, int speciesIndex, List<Organism> females, List<Organism> males)
    {
        foreach (Vector2 pair in pairs)
        {
            females[(int)pair.x].LookForMate(males[(int)pair.y]);
            males[(int)pair.y].LookForMate(females[(int)pair.x]);
        }
    }
    */
}
