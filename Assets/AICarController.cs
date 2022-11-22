using System.Collections;
using System.Linq;
using UnityEngine;
using Evo;

public class AICarController : Car
{
    [Range(0, 1)]
    public float timerMax = 0.2f;
    public bool forceRender;
    public EvolutionAgent agent;
    [SerializeField] Transform leftSensor;
    [SerializeField] Transform middleSensor;
    [SerializeField] Transform rightSensor;
    [SerializeField] LayerMask sensorMask;
    [SerializeField] AICarEngine engine;

    EvolutionGroup group;

    Vector3 startPos;
    Quaternion startRot;

    MeshRenderer[] meshRenderers;
    CheckpointManager checkpointManager;

    int scoreThreshold;

    Vector3 targetPos;

    protected override void Awake()
    {
        base.Awake();
        engine.Init(this);
    }

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
            bool renderEnabled = group.GetGeneration() > 1 && agent.IsElite || forceRender;
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

    public void SetTraining(bool training)
    {
        //agent
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

    public void EngineHitTrigger(Collider other)
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

    public void EngineCollided(Collision collision)
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
        RaycastHit hit;
        if (Raycast(middleSensor, out hit))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(middleSensor.position, hit.point);
        }
    }

    public string DNAToJson()
    {
        return JsonHelper.ArrayToJson(agent.DNA.Genes.Cast<float>().ToArray(), true);
    }
    
    public AICarInstance ToInstance()
    {
        return new AICarInstance(name, agent.DNA.Genes.Cast<float>().ToArray());
    }
}

public class AICarInstance
{
    string name = "Ai";
    float[] sensors;

    public AICarInstance(string name, float[] sensors)
    {
        this.name = name;
        this.sensors = sensors;
    }
}