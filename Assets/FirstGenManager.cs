using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FirstGenManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _populationPrefabList;


    //refactorizar para que siga el equilibrio de HW:
    private void Start()
    {
        int i = 0;       
        foreach (GameObject pop in _populationPrefabList)
        {
            Organism o = pop.GetComponent<Organism>();
            FirstGenCreator FCG = pop.GetComponent<FirstGenCreator>();
            SpeciesAttributes speciesAttributes = pop.GetComponent<SpeciesAttributes>();

            List<Vector3> newAlleleList = new List<Vector3>(); //x=locusIndex y=absoluteFrequency z=value
            List<int> newAlleleListNumOfOrganisms = new List<int>(); //lsita que guarda cuantos organismos tienen cada alelo
            foreach (Vector3 allele in FCG.AlleleList)
            {
                Vector3 newVector = new Vector3(allele.x, Mathf.RoundToInt(allele.y * speciesAttributes.InitialPopulation), allele.z);
                newAlleleList.Add(newVector);
                newAlleleListNumOfOrganisms.Add(0);
            }
            
            for (int k = 0; k < speciesAttributes.InitialPopulation; k++)
            {
                
                o._isFirstGen = true;
                o.FirstGenAttributes = o.GetComponent<FirstGenCreator>();

                foreach (LocusSO locusSO in FCG.LocusSOList)
                {
                    Locus newLocus = new Locus();
                    newLocus.Key = locusSO.Key;
                    newLocus.IsFunctional = locusSO.IsFunctional;
                    newLocus.AlleleValuesRange = locusSO.AlleleValuesRange;
                    newLocus.MutationMaxRange = locusSO.MutationMaxRange;
                    newLocus.MutationChance = locusSO.MutationChance;
                    newLocus.ChromosomeIndex = locusSO.ChromosomeIndex;

                    int l = -1;
                    int r = 0;
                    Vector2Int newAlleleValues = new Vector2Int();
                    foreach (Vector3 allele in newAlleleList)
                    {
                        l++;
                        if (allele.y <= newAlleleListNumOfOrganisms[l]) continue;

                        if (r == 0) newAlleleValues.x = Mathf.RoundToInt(allele.z);
                        if (r == 1) newAlleleValues.y = Mathf.RoundToInt(allele.z);
                        newAlleleListNumOfOrganisms[l]++;

                        r++;
                        if(r >= 2) break;
                    }

                    newLocus.AlleleValues = newAlleleValues;
                    FCG.LocusList.Add(newLocus);
                }

                foreach (Locus locus in FCG.LocusList)
                {
                    Debug.Log(locus.ChromosomeIndex);
                }
               Instantiate(pop, speciesAttributes.InitialPosition, Quaternion.identity, transform);
            }
            i++;
        }
    }
}
