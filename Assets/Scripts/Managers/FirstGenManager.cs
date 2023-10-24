using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FirstGenManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _populationPrefabList;


    private void Start()
    {
        int i = 0;
        foreach (GameObject pop in _populationPrefabList)
        {
            Organism o = pop.GetComponent<Organism>();
            FirstGenCreator FCG = pop.GetComponent<FirstGenCreator>();
            SpeciesAttributes speciesAttributes = pop.GetComponent<SpeciesAttributes>();

            o.FirstGenAttributes = FCG;

            List<List<Vector3>> genotypeList = new List<List<Vector3>>();
            List<List<int>> organismsWithGenotype = new List<List<int>>();

            int f = 0;
            foreach (LocusSO locusSO in FCG.LocusSOList)
            {
                genotypeList.Add(new List<Vector3>());

                List<Vector3> alleleListForF = new List<Vector3>();

                foreach (Vector3 allele in FCG.AlleleList)
                {
                    if (allele.x == f) alleleListForF.Add(allele);
                }

                genotypeList[f].AddRange(CalculateGenotipicFreqRel(alleleListForF)); // x=allele1  y=allele2  z=frequency Rel

                for (int d = 0; d < genotypeList[f].Count; d++)
                {
                    organismsWithGenotype.Add(new List<int>());
                    organismsWithGenotype[f].Add(0);
                }
                f++;
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
                    foreach (Vector3 genotype in genotypeList[t])
                    {
                        l++;
                        if ((genotype.z * speciesAttributes.InitialPopulation) < organismsWithGenotype[t][l]) continue;

                        newAlleleValues.x = (int)genotype.x;
                        newAlleleValues.y = (int)genotype.y;

                        organismsWithGenotype[t][l]++;
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

    List<Vector3> CalculateGenotipicFreqRel(List<Vector3> alleleList)
    {
        List<Vector3> genotypicFrequencies = new List<Vector3>(); // x=allele1  y=allele2  z=frequency

        int numAlleles = alleleList.Count;

        for (int i = 0; i < numAlleles; i++)
        {
            for (int j = i; j < numAlleles; j++)
            {
                float allele1 = alleleList[i].z;
                float allele2 = alleleList[j].z;

                float frequency = alleleList[i].y * alleleList[j].y;
                if (i != j) frequency = frequency * 2;
                Debug.Log("frequecy of " + alleleList[i].z + " " + alleleList[j].z + " is " + frequency);
                genotypicFrequencies.Add(new Vector3(allele1, allele2, frequency));
            }
        }

        return genotypicFrequencies;
    }


}