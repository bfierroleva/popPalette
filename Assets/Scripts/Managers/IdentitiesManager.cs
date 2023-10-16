using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentitiesManager : MonoBehaviour
{
    public List<Identity> AllIdentities = new List<Identity>();
    public List<Organism> LivingOrganisms = new List<Organism>();
    public IdentitiesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Identity CreateNewIdentity()
    {
        Identity newIdentity = new Identity();
        AllIdentities.Add(new Identity());
        newIdentity.ID = AllIdentities.Count;
        return newIdentity;
    }
}
