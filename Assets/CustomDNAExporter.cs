using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

#if UNITY_EDITOR
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

        if (group.GetBestFitness() >= threshold)
        {
            EvolutionAgent agent = group.agents.OrderByDescending(a => a.DNA.Fitness).First();
            agent.name = $"{agent.DNA.Fitness}({group.GetGeneration()})";
            agent.ExportDNA();
        }
    }
}
#endif
