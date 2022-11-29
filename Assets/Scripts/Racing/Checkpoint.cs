using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    CheckpointManager manager;

    public void SetManager(CheckpointManager manager)
    {
        this.manager = manager;
    }

    public void CarFound(Car carController)
    {
        manager.CarTraversedCheckpoint(this, carController, IsOnTheCorrectSide(carController));
    }

    public bool IsOnTheCorrectSide(Car carController)
    {
        Vector3 dirToCar = (carController.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToCar);
        return dot > 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }
}