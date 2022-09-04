using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    Dictionary<Car, int> carCheckpointIndicies = new Dictionary<Car, int>();
    [SerializeField] List<Checkpoint> checkpoints = new List<Checkpoint>();

    private void Awake()
    {
        GameObject checkpointsHolder = GameObject.Find("Checkpoints");
        checkpoints = checkpointsHolder.GetComponentsInChildren<Checkpoint>().ToList();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetManager(this);
        }

        Car[] allCars = FindObjectsOfType<Car>();

        foreach (Car controller in allCars)
        {
            carCheckpointIndicies.Add(controller, 0);
        }
    }

    public void CarTraversedCheckpoint(Checkpoint checkpoint, Car carController, bool followingPath)
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

        if (attemptedCheckpoint == 1 && lastCheckpoint == checkpoints.Count)
            carController.OnTraversedLastCheckpoint();
    }

    public void ResetCheckpointIndex(Car carController)
    {
        carCheckpointIndicies[carController] = 0;
    }
}