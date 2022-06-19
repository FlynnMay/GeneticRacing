using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

public class CustomDNAExporter : MonoBehaviour
{
    EvolutionGroup group;
    [Range(0f, 1f)]
    public float threshold = 1f;

    void Start()
    {
        group = GetComponent<EvolutionGroup>();    
    }

    void Update()
    {
        if (group.agents == null || group.agents.Length <= 0)
            return;

        if(group.GetBestFitness() >= threshold)
            group.agents.OrderByDescending(a => a.DNA.Fitness).First().ExportDNA();
    }
}
