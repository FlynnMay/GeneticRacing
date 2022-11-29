using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Evo;

public class AITest : MonoBehaviour
{
    public ManhattanDistanceFitnessFunction fitnessFunction;
    public Transform functionTarget;
    public uint rewardThreshold;
    EvolutionAgent agent;

    void Start()
    {
        agent = GetComponent<EvolutionAgent>();
        fitnessFunction.target = functionTarget;
    }

    public void CalculateFitness()
    {
        float value = fitnessFunction.GetValue(agent);
        float scoreTemp = Mathf.Clamp(agent.Score, int.MinValue, rewardThreshold);
        int modifier = scoreTemp >= 0 ? 1 : -1;
        float num = value * Mathf.Pow(Mathf.Abs(scoreTemp) / rewardThreshold, 2) * modifier;

        Debug.Log((Mathf.Clamp(num, -1, 1) + 1) / 2);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AITest))]
public class AITestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Test Fitness"))
            (target as AITest).CalculateFitness();
    }
}
#endif
