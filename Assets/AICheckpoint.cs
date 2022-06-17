using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICheckpoint : MonoBehaviour
{
    AICheckpointManager manager;

    public void SetManager(AICheckpointManager manager)
    {
        this.manager = manager;
    }

    public void CarFound(AICarController carController)
    {
        manager.CarTraversedCheckpoint(this, carController, IsOnTheCorrectSide(carController));
    }

    public bool IsOnTheCorrectSide(AICarController carController)
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