using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public string carName = "Car";
    public Rigidbody sphereRigidbody;
    public float speed = 200.0f;
    public float turnSpeed = 150.0f;
    float move;
    protected float vertical;
    protected float turn;
    protected bool canMove = true;

    public bool CanMove { get => canMove; set => canMove = value; }
    public bool Finished { get; protected set; } = false;

    protected void Awake()
    {
        GameObject parent = new GameObject($"{carName}: Car & Engine");
        parent.transform.SetParent(transform.parent);
        transform.SetParent(parent.transform);
        sphereRigidbody.transform.SetParent(parent.transform);
    }

    protected virtual void Update()
    {
        move = vertical * ((vertical > 0) ? speed : speed / 2);

        transform.position = sphereRigidbody.position;
        transform.Rotate(0, turn * turnSpeed * Time.deltaTime * vertical, 0, Space.World);
    }

    protected void FixedUpdate()
    {
        sphereRigidbody.AddForce(transform.forward * move, ForceMode.Acceleration);
    }

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
