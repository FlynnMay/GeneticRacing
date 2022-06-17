using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheckpointManager : MonoBehaviour
{
    Dictionary<AICarController, int> carCheckpointIndicies = new Dictionary<AICarController, int>();
    [SerializeField] List<AICheckpoint> checkpoints = new List<AICheckpoint>();

    private void Awake()
    {
        GameObject checkpointsHolder = GameObject.Find("Checkpoints");
        checkpoints = checkpointsHolder.GetComponentsInChildren<AICheckpoint>().ToList();

        foreach (AICheckpoint checkpoint in checkpoints)
        {
            checkpoint.SetManager(this);
        }

        AICarController[] allCars = FindObjectsOfType<AICarController>();

        foreach (AICarController controller in allCars)
        {
            carCheckpointIndicies.Add(controller, 0);
        }
    }

    public void CarTraversedCheckpoint(AICheckpoint checkpoint, AICarController carController, bool followingPath)
    {
        int lastCheckpoint = carCheckpointIndicies[carController];
        int attemptedCheckpoint = checkpoints.IndexOf(checkpoint) + 1;

        if (attemptedCheckpoint == lastCheckpoint + 1 && followingPath)
        {
            carCheckpointIndicies[carController]++;
            carController.OnTraversedCorrectCheckpoint(attemptedCheckpoint);
        }
        else if (attemptedCheckpoint == lastCheckpoint && !followingPath)
        {
            carCheckpointIndicies[carController]--;
            carController.OnTraversedWrongCheckpoint(attemptedCheckpoint, lastCheckpoint + 1);
        }
    }

    public void ResetCheckpointIndex(AICarController carController)
    { 
        carCheckpointIndicies[carController] = 0;
    }
}