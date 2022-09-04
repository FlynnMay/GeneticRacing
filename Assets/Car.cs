using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public string carName = "Car";
    protected bool canMove = true;
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool Finished { get; protected set; } = false;

    public virtual void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint) {}

    public virtual void OnTraversedCorrectCheckpoint(int checkpointIndex) { }
    public virtual void OnTraversedLastCheckpoint() 
    { 
        Finished = true;  
        GameManager.RaceManager.CarFinishedRace(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            checkpoint.CarFound(this);
        }
    }
}
