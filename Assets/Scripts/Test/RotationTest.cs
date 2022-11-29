using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    Quaternion targetRotation;
    public float speed = 1.0f;
    void Start()
    {
        targetRotation = Quaternion.Euler(0, Random.Range(0.0f, 1.0f) * 360, 0);
    }

    void Update()
    {
        Vector3 dir =  Vector3.RotateTowards(transform.forward, targetRotation * Vector3.forward, speed * Time.deltaTime, 0.0f);
        Debug.DrawRay(transform.position, dir);
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
