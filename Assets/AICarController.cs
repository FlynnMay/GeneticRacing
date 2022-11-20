using System.Collections;
using System.Linq;
using UnityEngine;
using Evo;

public class AICarController : Car
{
    [Range(0, 1)]
    public float timerMax = 0.2f;
    float timer = 0.0f;

    EvolutionGroup group;
    public EvolutionAgent agent;

    int genomeIndex = 0;

    Vector3 startPos;
    Quaternion startRot;

    MeshRenderer[] meshRenderers;
    CheckpointManager checkpointManager;

    int scoreThreshold;

    Vector3 targetPos;

    protected void Start()
    {
        group = FindObjectOfType<EvolutionGroup>();

        if (group != null)
            scoreThreshold = group.rewardThreshold;

        checkpointManager = FindObjectOfType<CheckpointManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        agent = GetComponent<EvolutionAgent>();

        startPos = transform.position;
        startRot = transform.rotation;

        agent.onResetEvent.AddListener(() =>
        {
            checkpointManager?.ResetCheckpointIndex(this);
            genomeIndex = 0;
            transform.position = startPos;
            transform.rotation = startRot;
            sphereRigidbody.gameObject.SetActive(true);
            timer = 0.0f;
        });
    }

    protected override void Update()
    {
        if (agent.DNA == null)
            return;

        if (agent.DNA.IsTraining)
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.enabled = group.GetGeneration() > 1 && agent.IsElite;

        if (!agent.IsAlive || !CanMove)
            return;

        int[] genes = agent.DNA.Genes.Cast<int>().ToArray();
        int gene = GetRotationFromGenes(genes);

        switch (gene)
        {
            case 0:
                turn = 0;
                vertical = 0;
                break;
            case 1:
                turn = 0;
                vertical = 1;
                break;
            case 2:
                turn = 0;
                vertical = -1;
                break;
            case 3:
                turn = -1;
                vertical = 1;
                break;
            case 4:
                turn = 1;
                vertical = 1;
                break;
            case 5:
                turn = -1;
                vertical = -1;
                break;
            case 6:
                turn = 1;
                vertical = -1;
                break;
            default:
                break;
        }

        base.Update();

        if (genomeIndex >= genes.Length)
        {
            agent.IsAlive = false;
            return;
        }

        timer += Time.deltaTime;
        if(timer > timerMax)
        {
            timer = 0.0f;
            genomeIndex++;
        }
    }

    private int GetRotationFromGenes(int[] genes)
    {
        return genes[genomeIndex];
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


    public void OnTriggerEnter(Collider other)
    {
        if (agent.DNA == null || !agent.DNA.IsTraining)
            return;

        if (other.CompareTag("Destination"))
        {
            if (agent.Score >= scoreThreshold)
            {
                agent.IsAlive = false;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (agent.DNA == null || !agent.DNA.IsTraining)
            return;

        if (collision.collider.CompareTag("Wall"))
        {
            agent.Penalise();
            agent.IsAlive = false;
            sphereRigidbody.velocity = Vector3.zero;
            sphereRigidbody.angularVelocity = Vector3.zero;
            sphereRigidbody.gameObject.SetActive(false);
            StopAllCoroutines();
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (agent == null || agent.DNA == null || !agent.IsAlive)
            return;

        Gizmos.DrawSphere(targetPos, 0.5f);
    }
}