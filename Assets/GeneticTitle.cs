using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;
using TMPro;

public class GeneticTitle : MonoBehaviour
{
    int lastGen = 0;
    public string title;
    public EvolutionGroup group;
    TextMeshProUGUI titleMesh;
    public CharFitnessFunction fitnessFunction;

    void Awake()
    {
        group = GetComponent<EvolutionGroup>();
        titleMesh = GetComponent<TextMeshProUGUI>();

        Setup();
    }

    public void Setup()
    {
        group.genomeSize = (uint)title.Length;
        fitnessFunction = (CharFitnessFunction)ScriptableObject.CreateInstance(typeof(CharFitnessFunction));
        fitnessFunction.target = title;
        group.fitnessFunction = fitnessFunction;
    }

    void Update()
    {
        if (group.GetGeneration() == lastGen)
            return;

        EvolutionAgent[] agents = group.agents;
        
        if (agents.Any(a => a.DNA == null) || agents.Length <= 0)
            return;

        EvolutionAgent[] elites = group.agents.Where(a => a.IsElite).ToArray();
        EvolutionAgent agent = elites.Length <= 0 ? agents[Random.Range(0, agents.Length)] : elites[Random.Range(0, elites.Length)];

        if(agents.Any(a => a.DNA.Fitness == 1))
            agent = agents.Where(a=>a.DNA.Fitness == 1).ToArray().FirstOrDefault();

        titleMesh.text = new string(agent.DNA.Genes.Cast<char>().ToArray());

        lastGen = group.GetGeneration();
    }
}
