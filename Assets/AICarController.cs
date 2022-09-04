using System.Collections;
using System.Linq;
using UnityEngine;
using Evo;

public class AICarController : Car
{
    public float speed = 5.0f;
    public int rotationCount = 27;
    public float[] rotations;

    [Range(0, 1)]
    public float timerMax = 0.2f;
    EvolutionGroup group;
    EvolutionAgent agent;

    int genomeRotIndex = 0;

    Vector3 startPos;
    Quaternion startRot;

    MeshRenderer[] meshRenderers;
    CheckpointManager checkpointManager;

    int scoreThreshold;

    Vector3 targetPos;

    void Start()
    {
        group = FindObjectOfType<EvolutionGroup>();

        if (group != null)
            scoreThreshold = group.rewardThreshold;

        checkpointManager = FindObjectOfType<CheckpointManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        agent = GetComponent<EvolutionAgent>();

        startPos = transform.position;
        startRot = transform.rotation;

        rotations = new float[rotationCount];

        float rotInteval = 360 / rotationCount;
        float rotSum = rotInteval;

        for (int i = 0; i < rotationCount; i++)
        {
            rotations[i] = rotSum;
            rotSum += rotInteval;
        }

        agent.onResetEvent.AddListener(() =>
        {
            //StopAllCoroutines();
            checkpointManager?.ResetCheckpointIndex(this);
            genomeRotIndex = 0;
            transform.position = startPos;
            transform.rotation = startRot;
            StartCoroutine(StartMovementOnDNAFound());
        });
        StartCoroutine(StartMovementOnDNAFound());
    }

    private IEnumerator StartMovementOnDNAFound()
    {
        while (agent.DNA == null)
            yield return null;

        OnTargetReached();
    }

    void Update()
    {
        if (agent.DNA == null)
            return;

        if (agent.DNA.IsTraining)
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.enabled = group.GetGeneration() > 1 && agent.IsElite;

        if (!agent.IsAlive || !CanMove)
            return;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        if (genomeRotIndex >= genes.Length)
        {
            agent.IsAlive = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!agent.IsAlive || !CanMove)
            return;

        if (Vector3.Distance(targetPos, transform.position) <= 0.2f)
            OnTargetReached();

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
    }

    private void OnTargetReached()
    {
        genomeRotIndex++;

        float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

        float y = GetRotationFromGenes(genes);

        Quaternion rot = Quaternion.Euler(0, y, 0);
        Vector3 dir = Vector3.RotateTowards(transform.forward, rot * Vector3.forward, speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(dir);

        targetPos = transform.position + dir;
    }

    private float GetRotationFromGenes(float[] genes)
    {
        int rotIndex = (int)((genes[genomeRotIndex] * (rotationCount - 1)) + 0.5f);
        float y = rotations[rotIndex];
        return y;
    }

    public override void OnTraversedWrongCheckpoint(int attemptedCheckpoint, int expectedCheckpoint)
    {
        agent.Penalise(3);
    }

    public override void OnTraversedCorrectCheckpoint(int checkpointIndex)
    {
        agent.Reward(3);
    }

    public override void OnTraversedLastCheckpoint()
    {
        base.OnTraversedLastCheckpoint();

        agent.IsAlive = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!agent.DNA.IsTraining)
            return;

        if (other.CompareTag("Destination"))
        {
            if (agent.Score >= scoreThreshold)
            {
                agent.IsAlive = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!agent.DNA.IsTraining)
            return;

        if (collision.collider.CompareTag("Wall"))
        {
            agent.Penalise();
            agent.IsAlive = false;
            StopAllCoroutines();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (agent == null || agent.DNA == null || !agent.IsAlive)
            return;

        Gizmos.DrawSphere(targetPos, 0.5f);
    }
}