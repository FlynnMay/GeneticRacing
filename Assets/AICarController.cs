using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Evo;
using System;

public class AICarController : MonoBehaviour
{
    EvolutionGroup group;
    EvolutionAgent agent;
    int genomeRotIndex = 0;
    int genomeSpeedIndex;
    float timer = 0;
    public float speed;
    public float speedModifier;
    Vector3 startPos;
    Quaternion startRot;
    MeshRenderer[] meshRenderers;
    AICheckpointManager checkpointManager;
    Rigidbody rb;
    int scoreThreshold;
    public int rotationCount = 27;
    public float[] rotations;

    public float[] speeds;
    public float maxForce = 100;
    public int forceCount = 8;

    [Range(0, 1)]
    public float timerMax = 0.2f;

    void Start()
    {
        group = FindObjectOfType<EvolutionGroup>();
        
        if (group != null)
            scoreThreshold = group.rewardThreshold;
        
        checkpointManager = FindObjectOfType<AICheckpointManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<EvolutionAgent>();

        timer = timerMax;
        startPos = transform.position;
        startRot = transform.rotation;

        rotations = new float[rotationCount];
        speeds = new float[forceCount];

        float rotInteval = 360 / rotationCount;
        float rotSum = rotInteval;

        for (int i = 0; i < rotationCount; i++)
        {
            rotations[i] = rotSum;
            rotSum += rotInteval;
        }

        float forceInterval = maxForce / forceCount;
        float forceSum = forceInterval;

        for (int i = 0; i < forceCount; i++)
        {
            speeds[i] = forceSum;
            forceSum += forceInterval;
        }

        agent.onResetEvent.AddListener(() =>
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            checkpointManager?.ResetCheckpointIndex(this);
            genomeRotIndex = 0;
            genomeSpeedIndex = agent.DNA.Genes.Length / 2;
            timer = timerMax;
            transform.position = startPos;
            transform.rotation = startRot;
        });
    }

    void Update()
    {
        Debug.Log(agent.Score);

        if (agent.DNA == null || group == null)
            return;

        //meshRenderer.material.color = agent.IsElite ? Color.green : Color.red;

        if (group.GetGeneration() > 1)
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.enabled = agent.IsElite;

        if (!agent.IsAlive)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            genomeRotIndex++;
            genomeSpeedIndex++;
            timer = timerMax;
            //addForce = true;
        }

        if (genomeRotIndex >= genes.Length / 2 || genomeSpeedIndex >= genes.Length)
        {
            agent.IsAlive = false;
            return;
        }

        int rotIndex = (int)((genes[genomeRotIndex] * (rotationCount - 1)) + 0.5f);
        float y = rotations[rotIndex];

        int speedIndex = (int)((genes[genomeSpeedIndex] * (forceCount - 1)) + 0.5f);
        speed = speeds[speedIndex];

        Quaternion rot = Quaternion.Euler(0, y, 0);
        Vector3 dir = Vector3.RotateTowards(transform.forward, rot * Vector3.forward, speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(dir);

    }

    private void FixedUpdate()
    {
        if (agent.DNA == null)
            return;

        if (agent.IsAlive /*&& addForce*/)
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        //addForce = false;
    }

    public void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint)
    {
        agent.Penalise(3);
    }

    public void OnTraversedCorrectCheckpoint(int checkpointIndex)
    {
        agent.Reward(3);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Wall"))
        {
            agent.Penalise();
            agent.IsAlive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            if (agent.Score >= scoreThreshold)
                agent.IsAlive = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            AICheckpoint checkpoint = other.GetComponent<AICheckpoint>();
            checkpoint.CarFound(this);
        }
    }
}