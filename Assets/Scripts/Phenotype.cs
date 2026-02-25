using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phenotype : MonoBehaviour
{
    private new SpriteRenderer renderer;

    public Color PhenotypeColor;
    //public Vector3 PhenotypeValue; // x=R y=G z=B
    public void SetPhenotype(Identity identity)
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = Color.black;
        PhenotypeColor = Color.black;
        foreach (ChromosomePair ChromosomePair in identity.ThisGenotype.Karyotype)
        {
            foreach (Locus locus in ChromosomePair.LocusList)
            {
                if (!locus.IsFunctional) continue;
                
                if (locus.Key == "colorR")
                {
                    renderer.color = new Color((((float)locus.AlleleValues.x + (float)locus.AlleleValues.y) / 2) / (float)locus.AlleleValuesRange.y, renderer.color.g, renderer.color.b, renderer.color.a);
                }
                if (locus.Key == "colorG")
                {
                    renderer.color = new Color(renderer.color.r, (((float)locus.AlleleValues.x + (float)locus.AlleleValues.y) / 2) / (float)locus.AlleleValuesRange.y, renderer.color.b, renderer.color.a);
                }
                if (locus.Key == "colorB")
                {
                    renderer.color = new Color(renderer.color.r, renderer.color.g, (((float)locus.AlleleValues.x + (float)locus.AlleleValues.y) / 2) / (float)locus.AlleleValuesRange.y, renderer.color.a);
                }
            }
        }
        PhenotypeColor = renderer.color;
    }
}
