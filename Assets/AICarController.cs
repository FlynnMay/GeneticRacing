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
    public float speed = 5.0f;
    
    Vector3 startPos;
    Quaternion startRot;
    
    MeshRenderer[] meshRenderers;
    AICheckpointManager checkpointManager;
    
    int scoreThreshold;
    public int rotationCount = 27;
    public float[] rotations;

    public float[] speeds;
    float maxSpeed = 100.0f;
    public int speedCount = 8;

    [Range(0, 1)]
    public float timerMax = 0.2f;

    float lifetime = 0;

    Vector3 targetPos;

    void Start()
    {
        group = FindObjectOfType<EvolutionGroup>();
        
        if (group != null)
            scoreThreshold = group.rewardThreshold;
        
        checkpointManager = FindObjectOfType<AICheckpointManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        agent = GetComponent<EvolutionAgent>();

        timer = timerMax;
        startPos = transform.position;
        startRot = transform.rotation;

        rotations = new float[rotationCount];
        speeds = new float[speedCount];

        float rotInteval = 360 / rotationCount;
        float rotSum = rotInteval;

        for (int i = 0; i < rotationCount; i++)
        {
            rotations[i] = rotSum;
            rotSum += rotInteval;
        }

        float speedInterval = maxSpeed / speedCount;
        float speedSum = speedInterval;

        for (int i = 0; i < speedCount; i++)
        {
            speeds[i] = speedSum;
            speedSum += speedInterval;
        }

        agent.onResetEvent.AddListener(() =>
        {
            //StopAllCoroutines();
            lifetime = 0;
            checkpointManager?.ResetCheckpointIndex(this);
            genomeRotIndex = 0;
            genomeSpeedIndex = agent.DNA.Genes.Length / 2;
            timer = timerMax;
            transform.position = startPos;
            transform.rotation = startRot;
            StartCoroutine(StartMovementOnDNAFound());
        });
        StartCoroutine(StartMovementOnDNAFound());
    }

    private IEnumerator StartMovementOnDNAFound()
    {
        yield return new WaitUntil(() => agent.DNA != null);

        OnTargetReached();
    }

    void Update()
    {
        if (agent.DNA == null || group == null)
            return;

        if (group.GetGeneration() > 1)
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.enabled = agent.IsElite;

        if (!agent.IsAlive)
            return;

        lifetime += Time.deltaTime;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        if (genomeRotIndex >= genes.Length / 2 || genomeSpeedIndex >= genes.Length)
        {
            agent.IsAlive = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!agent.IsAlive)
            return;

        if (Vector3.Distance(targetPos, transform.position) <= 0.1f)
            OnTargetReached();

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
    }

    private void OnTargetReached()
    {
        genomeRotIndex++;
        genomeSpeedIndex++;
        timer = timerMax;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        float y = GetRotationFromGenes(genes);

        Quaternion rot = Quaternion.Euler(0, y, 0);
        Vector3 dir = Vector3.RotateTowards(transform.forward, rot * Vector3.forward, speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(dir);

        targetPos = transform.position + dir;
    }

    private void CalculateSpeedFromGenes(float[] genes)
    {
        int speedIndex = (int)((genes[genomeSpeedIndex] * (speedCount - 1)) + 0.5f);
        speed = speeds[speedIndex];
    }

    private float GetRotationFromGenes(float[] genes)
    {
        int rotIndex = (int)((genes[genomeRotIndex] * (rotationCount - 1)) + 0.5f);
        float y = rotations[rotIndex];
        return y;
    }

    public void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint)
    {
        agent.Penalise(3);
    }

    public void OnTraversedCorrectCheckpoint(int checkpointIndex)
    {
        agent.Reward(3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            if (agent.Score >= scoreThreshold)
            {
                agent.IsAlive = false;
            }
        }

        if (other.CompareTag("Wall"))
        {
            agent.Penalise();
            agent.IsAlive = false;
            StopAllCoroutines();
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

    private void OnDrawGizmosSelected()
    {
        if (agent == null)
            return;

        if (agent.DNA == null || group == null)
            return;

        if (!agent.IsAlive)
            return;

        Gizmos.DrawSphere(targetPos, 0.5f);
    }
}