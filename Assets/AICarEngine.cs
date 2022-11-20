using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarEngine : MonoBehaviour
{
    AICarController controller;
    Rigidbody rb;

    void Awake()
    {
        controller = GetComponentInParent<AICarController>();
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator Start()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while(controller.agent == null)
            yield return null;

        controller.agent.onResetEvent.AddListener(() =>
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = startPos;
            transform.rotation = startRot;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        controller.OnTriggerEnter(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        controller.OnCollisionEnter(collision);
    }
}
