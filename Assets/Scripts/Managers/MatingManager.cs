using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MatingManager : MonoBehaviour
{
    [SerializeField] List<SpeciesSO> _speciesList;

    private void Start()
    {
        int i = 0;
        foreach (SpeciesSO species in _speciesList) 
        {
            StartCoroutine(ReproductionCycle(i));
            i++;
        }
    }

    IEnumerator ReproductionCycle(int speciesIndex)
    {
        yield return new WaitForSeconds(_speciesList[speciesIndex].ReprductionCycleInSeconds);
        PairMates(speciesIndex);
        ReproductionCycle(speciesIndex);
    }

    void PairMates(int speciesIndex)
    {
        List<Organism> organismOfThisSpecies = new List<Organism>();
        List<bool> matedOrganisms = new List<bool>();
        Organism[] allOrganisims = FindObjectsOfType<Organism>();
        foreach (Organism o in allOrganisims)
        {
            if (o.SpeciesAttributes.SpeciesName == _speciesList[speciesIndex].SpeciesName) { organismOfThisSpecies.Add(o); matedOrganisms.Add(false); }
        }
        for (int i = 0; i < organismOfThisSpecies.Count; i++)
        {
            if (!matedOrganisms[i])
            {
                LocateMate(speciesIndex, organismOfThisSpecies, matedOrganisms, i, new List<int> { i });
            }
            else Debug.Log(organismOfThisSpecies[i] + " has already mated");      
        }
        //Debug.LogError("pause");
    }

    void LocateMate(int speciesIndex, List<Organism> organismOfThisSpecies, List<bool> matedOrganisms, int indexOfOrganismLooking, List<int> ignore)
    {
        int closestPossibleMate = -1;
        float closestDistance1 = _speciesList[speciesIndex].LookForMateViewDistance;
        for (int j = 0; j < organismOfThisSpecies.Count; j++)
        {
            bool shouldIgnore = false;
            //Debug.Log(indexOfOrganismLooking + " ignora a " + ignore[0]);
            if (matedOrganisms[j]) shouldIgnore = true;
            foreach (int i in ignore) if (i == j) shouldIgnore = true;
            if (!shouldIgnore)
            {
                float distance = Vector3.Distance(organismOfThisSpecies[indexOfOrganismLooking].transform.position, organismOfThisSpecies[j].transform.position);

                if (distance < closestDistance1)
                {
                    Debug.Log("new closest: " + organismOfThisSpecies[j].transform.gameObject.name);
                    closestDistance1 = distance;
                    closestPossibleMate = j;
                }
            }
        }
        if (closestPossibleMate != -1) Debug.Log(organismOfThisSpecies[indexOfOrganismLooking].transform.gameObject.name + " fancies " + organismOfThisSpecies[closestPossibleMate].transform.gameObject.name);

        if (closestPossibleMate == -1) { matedOrganisms[indexOfOrganismLooking] = true; return; }


        int closestPossibleMateOfTheclosestPossibleMate = -1;
        float closestDistance2 = _speciesList[speciesIndex].LookForMateViewDistance;
        for (int k = 0; k < organismOfThisSpecies.Count; k++)
        {
            if (matedOrganisms[k]) continue;
            if (k != closestPossibleMate)
            {
                float distance = Vector3.Distance(organismOfThisSpecies[closestPossibleMate].transform.position, organismOfThisSpecies[k].transform.position);

                if (distance < closestDistance2)
                {
                    closestDistance2 = distance;
                    closestPossibleMateOfTheclosestPossibleMate = k;
                }
            }                
        }
        Debug.Log(organismOfThisSpecies[closestPossibleMate].transform.gameObject.name + " is closest to " + organismOfThisSpecies[closestPossibleMateOfTheclosestPossibleMate].transform.gameObject.name);

        if (closestPossibleMateOfTheclosestPossibleMate == indexOfOrganismLooking) 
        {
            Debug.Log(organismOfThisSpecies[indexOfOrganismLooking].transform.gameObject.name + " mated with " + organismOfThisSpecies[closestPossibleMate].transform.gameObject.name);

            //organismOfThisSpecies[indexOfOrganismLooking].Reproduce(organismOfThisSpecies[closestPossibleMate]);
            //organismOfThisSpecies[closestPossibleMate].Reproduce(organismOfThisSpecies[indexOfOrganismLooking]);
            matedOrganisms[indexOfOrganismLooking] = true;
            matedOrganisms[closestPossibleMate] = true;
        }
        else
        {
            ignore.Add(closestPossibleMate);
            LocateMate(speciesIndex, organismOfThisSpecies, matedOrganisms, indexOfOrganismLooking, ignore);
        }     
    }
}
