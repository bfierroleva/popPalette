using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class SavePopData : MonoBehaviour
{
    public string CollectPopDataFormated()
    {
        string newPopDataFormated = FormatGenepopData(CollectPopData());
        return newPopDataFormated;
    }

    public List<List<List<string>>> CollectPopData()
    {
        List<List<List<string>>> newPopDataMatrix = new List<List<List<string>>>();
        int popIndex = 0;
        int currentGen = GetComponent<GenerationManager>().CurrentGen;
        foreach (List<PopulationGen> population in GetComponent<PopulationManager>().PopulationGenList)
        {
            newPopDataMatrix.Add(new List<List<string>>());
            bool createdHeaders = false;
            int organismIndex = 0;
            foreach (Organism o in population[currentGen].OrganismList)
            {
                List<string> locusValueList = new List<string>();
                if (!createdHeaders) newPopDataMatrix[popIndex].Add(new List<string> { o.SpeciesAttributes.PopulationKey });
                foreach (ChromosomePair chromosome in o.ThisIdentity.ThisGenotype.Karyotype)
                {
                    foreach (Locus locus in chromosome.LocusList)
                    {
                        if (!createdHeaders) newPopDataMatrix[popIndex][0].Add(locus.Key);
                        String newAllele1 = locus.AlleleValues.x.ToString();
                        String newAllele2 = locus.AlleleValues.y.ToString();
                        newAllele1 = CorrectDigits(newAllele1, locus.AlleleValuesRange.y.ToString());
                        newAllele2 = CorrectDigits(newAllele2, locus.AlleleValuesRange.y.ToString());
                        locusValueList.Add(newAllele1 + newAllele2);
                    }

                }

                newPopDataMatrix[popIndex].Add(new List<string> { o.SpeciesAttributes.PopulationKey + "_" + (organismIndex + 1).ToString() });
                newPopDataMatrix[popIndex][organismIndex + 1].AddRange(locusValueList);
                createdHeaders = true;
                organismIndex++;
            }
            popIndex++;
        }


        return newPopDataMatrix;

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

    string FormatGenepopData(List<List<List<string>>> populationData)
    {
        StringBuilder genepopText = new StringBuilder();

        // Extract species name and loci names
        //string speciesKey = populationData[0][0][0];
        List<string> lociNames = populationData[0][0].GetRange(1, populationData[0][0].Count - 1);

        // Append species name
        genepopText.AppendLine("Data extracted from PopPalette");

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

    
}
