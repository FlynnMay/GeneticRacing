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
    
    Vector3 startPos;
    Quaternion startRot;
    
    MeshRenderer[] meshRenderers;
    AICheckpointManager checkpointManager;
    
    int scoreThreshold;
    public int rotationCount = 27;
    public float[] rotations;

    public float[] speeds;
    public float maxSpeed = 100;
    public int speedCount = 8;
    event Action onLerpComplete;

    [Range(0, 1)]
    public float timerMax = 0.2f;

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
            checkpointManager?.ResetCheckpointIndex(this);
            genomeRotIndex = 0;
            genomeSpeedIndex = agent.DNA.Genes.Length / 2;
            timer = timerMax;
            StopAllCoroutines();
            transform.position = startPos;
            transform.rotation = startRot;
            StartCoroutine(StartLerpWhenDNAFound());
        });

        onLerpComplete += AICarController_onLerpComplete;
        StartCoroutine(StartLerpWhenDNAFound());
    }

    void AICarController_onLerpComplete()
    {
        genomeRotIndex++;
        genomeSpeedIndex++;
        timer = timerMax;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        StartLerp(genes);
    }

    IEnumerator StartLerpWhenDNAFound()
    {
        while (agent.DNA == null)
            yield return null;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        StartLerp(genes);
    }

    private void StartLerp(float[] genes)
    {
        if (genomeRotIndex < genes.Length / 2)
        {
            float y = GetRotationFromGenes(genes);
            Quaternion rot = Quaternion.Euler(0, y, 0);
            Vector3 dir = rot * Vector3.forward;
            Vector3 target = transform.position + dir * 5;
            StartCoroutine(Lerp(target, rot, speed));
        }
    }

    void Update()
    {
        Debug.Log(agent.Score);

        if (agent.DNA == null || group == null)
            return;

        if (group.GetGeneration() > 1)
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.enabled = agent.IsElite;

        if (!agent.IsAlive)
            return;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        if (genomeRotIndex >= genes.Length / 2 || genomeSpeedIndex >= genes.Length)
        {
            agent.IsAlive = false;
            return;
        }

        float y = GetRotationFromGenes(genes);
        CalculateSpeedFromGenes(genes);

        Quaternion rot = Quaternion.Euler(0, y, 0);
        Vector3 dir = Vector3.RotateTowards(transform.forward, rot * Vector3.forward, speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(dir);
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

    IEnumerator Lerp(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float timeElapsed = 0.0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        onLerpComplete?.Invoke();
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

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            if (agent.Score >= scoreThreshold)
                agent.IsAlive = false;
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
}