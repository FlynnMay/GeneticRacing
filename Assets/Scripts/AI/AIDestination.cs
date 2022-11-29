using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

public class AIDestination : MonoBehaviour
{
    public ManhattanDistanceFitnessFunction fitnessFunction;
    
    void Start()
    {
        fitnessFunction.target = transform;    
    }
}
