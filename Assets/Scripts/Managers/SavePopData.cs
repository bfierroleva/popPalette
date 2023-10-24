using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;


//refactorizar para varias poblaciones:
public class SavePopData : MonoBehaviour
{
    [SerializeField] private string _speciesToSave;

    public string CollectPopData()
    {
        List<List<List<string>>> popDataMatrix = new List<List<List<string>>>();
        int i = 0;
        foreach (Population population in GetComponent<PopulationManager>().populationList)
        {
            if (_speciesToSave != population.SpeciesKey) continue;
            popDataMatrix.Add(new List<List<string>>());
            bool createdHeaders = false;
            int j = 0;
            foreach (Organism o in population.OrganismList)
            {
                List<string> locusValueList = new List<string>();
                if (!createdHeaders) popDataMatrix[i].Add(new List<string> { o.SpeciesAttributes.SpeciesKey });
                foreach (ChromosomePair chromosome in o.ThisIdentity.ThisGenotype.Karyotype)
                {
                    foreach (Locus locus in chromosome.LocusList)
                    {
                        if (!createdHeaders) popDataMatrix[i][0].Add(locus.Key);
                        String newAllele1 = locus.AlleleValues.x.ToString();
                        String newAllele2 = locus.AlleleValues.y.ToString();
                        newAllele1 = CorrectDigits(newAllele1, locus.AlleleValuesRange.y.ToString());
                        newAllele2 = CorrectDigits(newAllele2, locus.AlleleValuesRange.y.ToString());
                        locusValueList.Add(newAllele1 + newAllele2);
                    }

                }

                popDataMatrix[i].Add(new List<string> { o.SpeciesAttributes.PopulationKey + "_" + (j + 1).ToString() });
                popDataMatrix[i][j + 1].AddRange(locusValueList);
                createdHeaders = true;
                j++;
            }
            i++;
        }

        
        return FormatGenepopData(popDataMatrix);
    }
    string FormatGenepopData(List<List<List<string>>> populationData)
    {
        StringBuilder genepopText = new StringBuilder();

        // Extract species name and loci names
        string speciesKey = populationData[0][0][0];
        List<string> lociNames = populationData[0][0].GetRange(1, populationData[0][0].Count - 1);

        // Append species name
        genepopText.AppendLine(speciesKey);

        // Append loci names
        foreach (string locusName in lociNames)
        {
            genepopText.AppendLine(locusName);
        }

        foreach (List<List<string>> population in populationData)
        {
            genepopText.AppendLine("POP");

            int j = 0;
            foreach (List<String> observation in population)
            {
                if (j == 0) { j++; continue; }
                genepopText.Append(observation[0] + ",\t");
                int k = 0;
                foreach (string locusValues in observation)
                {
                    if (k == 0) { k++; continue; }
                    genepopText.Append(locusValues + "\t");

                    k++;
                }
                genepopText.AppendLine();
                j++;
            }
        }
        
        return genepopText.ToString();
    }

    string CorrectDigits(string allele, string maxRange)
    {
        if (allele.Length < maxRange.Length)
        {
            int zeroesToAdd = maxRange.Length - allele.Length;
            for (int i = 0; i < zeroesToAdd; i++)
            {
                allele = new string("0" + allele);
            }          
        }
        return allele;
    }
}
