using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;


//refactorizar para varias poblaciones:
public class SavePopData : MonoBehaviour
{
    [SerializeField] SpeciesSO _speciesToSave;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Call your method here
            Debug.Log(CollectPopData());
        }
    }

    string CollectPopData()
    {
        List<Organism> pop = new List<Organism>();
        List<List<string>> popDataMatrix = new List<List<string>>();
        bool createdHeaders = false;
        int i = 0;
        foreach (Organism o in FindObjectsOfType<Organism>()) 
        {
            if (o.SpeciesAttributes.SpeciesKey != _speciesToSave.SpeciesKey) continue;
            List<string> locusValueList = new List<string>();
            if (!createdHeaders) popDataMatrix.Add(new List<string> { o.SpeciesAttributes.SpeciesKey });
            foreach (ChromosomePair chromosome in o.ThisIdentity.ThisGenotype.Karyotype)
            {
                foreach (Locus locus in chromosome.LocusList)
                {
                    if (!createdHeaders) popDataMatrix[0].Add(locus.Key);
                    String newAllele1 = locus.AlleleValues.x.ToString();
                    String newAllele2 = locus.AlleleValues.y.ToString();
                    newAllele1 = CorrectDigits(newAllele1, locus.AlleleValuesRange.y.ToString());
                    newAllele2 = CorrectDigits(newAllele2, locus.AlleleValuesRange.y.ToString());
                    locusValueList.Add(newAllele1 + newAllele2);                   
                }
                
            }

            popDataMatrix.Add(new List<string> { o.SpeciesAttributes.SpeciesKey + "_" + (i + 1).ToString() });
            popDataMatrix[i + 1].AddRange(locusValueList);
            createdHeaders = true;
            i++;
        }
        return FormatGenepopData(popDataMatrix);
    }
    string FormatGenepopData(List<List<string>> populationData)
    {
        StringBuilder genepopText = new StringBuilder();

        // Extract species name and loci names
        string speciesName = populationData[0][0];
        List<string> lociNames = populationData[0].GetRange(1, populationData[0].Count - 1);

        // Append species name
        genepopText.AppendLine(speciesName);

        // Append loci names
        foreach (string locusName in lociNames)
        {
            genepopText.AppendLine(locusName);
        }

        // Append an empty line
        genepopText.AppendLine("POP");

        int i = 0;
        foreach (List<String> observation in populationData)
        {
            if (i == 0) { i++; continue; }
            genepopText.Append(observation[0] + ",\t");
            int j = 0;
            foreach (string locusValues in observation)
            {
                if (j == 0) { j++; continue; }
                genepopText.Append(locusValues + "\t");

                j++;
            }
            genepopText.AppendLine();
            i++;
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
