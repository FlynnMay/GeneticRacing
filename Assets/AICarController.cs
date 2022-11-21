using System.Collections;
using System.Linq;
using UnityEngine;
using Evo;

public class AICarController : Car
{
    [Range(0, 1)]
    public float timerMax = 0.2f;
    public EvolutionAgent agent;
    [SerializeField] Transform leftSensor;
    [SerializeField] Transform rightSensor;
    [SerializeField] LayerMask sensorMask;

    EvolutionGroup group;

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
        {
            scoreThreshold = group.rewardThreshold;

            for (int i = 0; i < extraVfx.Length; i++)
                extraVfx[i].SetActive(!GameManager.Instance.IsTraining);
        }

        checkpointManager = FindObjectOfType<CheckpointManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        agent = GetComponent<EvolutionAgent>();

        startPos = transform.position;
        startRot = transform.rotation;

        agent.onResetEvent.AddListener(() =>
        {
            checkpointManager?.ResetCheckpointIndex(this);
            transform.position = startPos;
            transform.rotation = startRot;
            sphereRigidbody.gameObject.SetActive(true);
            Finished = false;
            KeepInView = true;
        });
    }

    protected override void Update()
    {
        if (agent.DNA == null)
            return;

        if (agent.DNA.IsTraining)
        {
            bool renderEnabled = group.GetGeneration() > 1 && agent.IsElite;
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.enabled = renderEnabled;
        }

        if (!agent.IsAlive || !CanMove)
            return;

        if (Finished)
            agent.IsAlive = false;

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (agent.DNA != null)
        {
            float[] genes = agent.DNA.Genes.Cast<float>().ToArray();

            turn = 0;
            vertical = 1;
            float tempTurn = 0;

            RaycastHit hit;
            if (Raycast(leftSensor, out hit))
            {
                tempTurn += hit.distance < genes[1] ? 1 : 0;
            }

            if (Raycast(rightSensor, out hit))
            {
                tempTurn += hit.distance < genes[2] ? -1 : 0;
            }

            turn = tempTurn;
        }

        base.FixedUpdate();
    }

    private bool Raycast(Transform sensor, out RaycastHit hit)
    {
        return Physics.Raycast(sensor.position, sensor.forward, out hit, float.PositiveInfinity, sensorMask);
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
            KeepInView = false;
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