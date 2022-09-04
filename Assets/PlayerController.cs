using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Car
{
    public Rigidbody sphereRigidbody;
    public float speed = 200.0f;
    public float turnSpeed = 150.0f;
    float move;
    float turn;

    private void Awake()
    {
        sphereRigidbody.transform.SetParent(null);
    }

    private void Update()
    {
        float moveInput = Input.GetAxisRaw("Vertical");
        move = moveInput * ((moveInput > 0) ? speed : speed / 2); 

        turn = Input.GetAxisRaw("Horizontal");

        transform.position = sphereRigidbody.position;
        transform.Rotate(0, turn * turnSpeed * Time.deltaTime * moveInput, 0, Space.World);
    }

    private void FixedUpdate()
    {
        sphereRigidbody.AddForce(transform.forward * move, ForceMode.Acceleration);
    }

    public override void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint)
    {
    }

    public override void OnTraversedCorrectCheckpoint(int checkpointIndex)
    {
    }

    public override void OnTraversedLastCheckpoint()
    {
        sphereRigidbody.velocity = Vector3.zero;
        speed = 0.0f;
    }
}
