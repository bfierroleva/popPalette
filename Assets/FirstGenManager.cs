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

            o.FirstGenAttributes = FCG;

            List<Vector3> newAlleleList = new List<Vector3>(); //x=locusIndex y=absoluteFrequency z=value
            List<int> newAlleleListNumOfAllelesOnPop = new List<int>(); //lsita que guarda cuantos alelos hay ya en la poblacion
            foreach (Vector3 allele in FCG.AlleleList)
            {
                Vector3 newVector = new Vector3(allele.x, Mathf.RoundToInt(allele.y * speciesAttributes.InitialPopulation * 2), allele.z);
                Debug.Log(newVector.y);
                newAlleleList.Add(newVector);
                newAlleleListNumOfAllelesOnPop.Add(0);
            }
            
            for (int k = 0; k < speciesAttributes.InitialPopulation; k++)
            {
                GameObject newOrganisimObject = Instantiate(pop, speciesAttributes.InitialPosition, Quaternion.identity, transform);
                newOrganisimObject.name = (k + 1).ToString();
                Organism newOrganisim = newOrganisimObject.GetComponent<Organism>();
                FirstGenCreator newFCG = newOrganisimObject.GetComponent<FirstGenCreator>();

                newOrganisim._isFirstGen = true;
                newOrganisim.FirstGenAttributes = newFCG;
                

                int t = 0;
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
                    Vector2Int newAlleleValues = new Vector2Int();
                    foreach (Vector3 allele in newAlleleList)
                    {
                        l++;
                        if (allele.x != t) continue;
                        if (allele.y < newAlleleListNumOfAllelesOnPop[l]) continue;

                        newAlleleValues.x = (int)allele.z;

                        newAlleleListNumOfAllelesOnPop[l]++;
                        break;
                    }
                    l = -1;
                    foreach (Vector3 allele in newAlleleList)
                    {
                        l++;
                        if (allele.x != t) continue;
                        if (allele.y < newAlleleListNumOfAllelesOnPop[l]) continue;

                        newAlleleValues.y = (int)allele.z;

                        newAlleleListNumOfAllelesOnPop[l]++;
                        break;
                    }

                    newLocus.AlleleValues = newAlleleValues;
                    newFCG.LocusList.Add(newLocus);
                    t++;
                }
                newOrganisim.OnStart();
                
            }
            i++;
            
        }

    }

}
