using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Car
{

    protected override void Update()
    {
        if (!CanMove)
            return;

        vertical = Input.GetAxisRaw("Vertical");
        turn = Input.GetAxisRaw("Horizontal");

        base.Update();
    }

    public override void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint)
    {
    }

    public override void OnTraversedCorrectCheckpoint(int checkpointIndex)
    {
    }

    public override void OnTraversedLastCheckpoint()
    {
        base.OnTraversedLastCheckpoint();

        sphereRigidbody.velocity = Vector3.zero;
        speed = 0.0f;
    }
}
