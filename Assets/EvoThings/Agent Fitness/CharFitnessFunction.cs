using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

[CreateAssetMenu(fileName = "Char Fitness Function", menuName = "EvoCustom/FitnessFunctions/CharCustomFitness")]
public class CharFitnessFunction : FitnessFunction
{
    public string target = "";
    //Returns the fitness value, the final value must be between 0 and 1.
    public override float GetValue(EvolutionAgent agent)
    {
        int match = 0;
        for (int i = 0; i < target.Length; i++)
        {
            if(target[i] == (char)agent.DNA.Genes[i])
                match++;
        }

        return (float)match / target.Length;
    }
}