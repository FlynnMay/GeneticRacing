using System.Collections;
using System.Linq;
using UnityEngine;
using Evo;
using System;

public class AICarController : Car
{
    [Range(0, 1)]
    public float timerMax = 0.2f;
    public bool forceRender;
    public EvolutionAgent agent;
    public event Action OnReady;

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
    CarColour colour;

    protected override void Awake()
    {
        base.Awake();
        engine.Init(this);
        colour = GetComponentInChildren<CarColour>();
    }

    protected void Start()
    {
        FindAgent();
        group = agent.group;

        if (group != null)
        {
            scoreThreshold = group.rewardThreshold;

            for (int i = 0; i < extraVfx.Length; i++)
                extraVfx[i].SetActive(!GameManager.Instance.IsTraining);
        }

        checkpointManager = FindObjectOfType<CheckpointManager>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

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

        StartCoroutine(WaitForAgentDNA());
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        engine.transform.position = pos;
        engine.transform.rotation = rot;
        transform.position = pos;
        transform.rotation = rot;
    }

    public void FindAgent()
    {
        if (agent == null)
            agent = GetComponent<EvolutionAgent>();
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

    IEnumerator WaitForAgentDNA()
    {
        while (agent == null)
            yield return null;

        OnReady?.Invoke();
    }

    public void SetTraining(bool training)
    {
        agent.SetTraining(training);
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

    public AICarInstance ToInstance()
    {
        return new AICarInstance(name, agent.DNA.Genes.Cast<float>().ToArray(), agent.group.GetGeneration(), colour.GetIndex());
    }

    public void FromInstance(AICarInstance instance)
    {
        name = instance.name;
        carName = instance.name;
        DNA<float> dna = ScriptableObject.CreateInstance<EvoDNAFloat>();
        dna.genes = instance.genes;
        agent.SetAndApplyDNA(dna);
        colour.SetColourFromIndex(instance.colourIndex);
    }
}

[Serializable]
public class AICarInstance
{
    public string name = "Ai";
    public int generations = 0;
    public float[] genes;
    public int colourIndex = 0;

    public AICarInstance(string name, float[] genes, int generations, int colourIndex)
    {
        this.name = name;
        this.genes = genes;
        this.generations = generations;
        this.colourIndex = colourIndex;
    }
}