using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public virtual void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint) {}
    public virtual void OnTraversedCorrectCheckpoint(int checkpointIndex) { }
    public virtual void OnTraversedLastCheckpoint() { }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            checkpoint.CarFound(this);
        }
    }
}
