using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
}
