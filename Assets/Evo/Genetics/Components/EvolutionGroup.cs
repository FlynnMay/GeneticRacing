using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Evo
{
    /// <summary>
    /// Controls the Genetic Algorithm and manages all the agents
    /// </summary>
    public class EvolutionGroup : MonoBehaviour, IEvolutionInstructions
    {
        [Serializable]
        private class TimerSettings
        {
            /// <summary>
            /// Determines which Time system is used
            /// </summary>
            public enum TimerType
            {
                UnityEngine,
                GeneticTimer
            }

            [Tooltip("If checked the group will evolve when the timer reaches the max time")]
            public bool useTimer = true;

            [Tooltip("Determines which Time system is used, UnityEngine (UnityEngine.Time) or\n" +
                "GeneticTime (scales seperatley from UnityEngine.Time, WARNING: May cause issues with physics)")]
            public TimerType timerType = TimerType.UnityEngine;

            [Tooltip("The rate at which the timer changes")]
            [Range(0f, 100f)] public float timeScale = 1.0f;

            [Tooltip("The point at which the timer resets")]
            public float timerMax = 30.0f;

            [Tooltip("The Generations Elapsed Time")]
            [ReadOnly] public float timer = 0.0f;
        }

        /// <summary>
        /// Used to generate custom random values for genetic algorithm
        /// </summary>
        [Header("Configuration")]
        [Tooltip("Used to generate custom random values for genetic algorithm")]
        public DNAValueGenerator valueGenerator;

        /// <summary>
        /// Used to calclulate fitness for genetic algorithm
        /// </summary>
        [Tooltip("Used to calclulate fitness for genetic algorithm")]
        public FitnessFunction fitnessFunction;

        /// <summary>
        /// Determines the chance of a agent to be mutated, clamped between 0.0f and 1.0f
        /// </summary>
        [SerializeField]
        [Tooltip("Determines the chance of a agent to be mutated")]
        [Range(0.0f, 1.0f)]
        float mutationRate = 0.01f;

        /// <summary>
        /// Determines the number of surperior agents which live on to the next generation
        /// </summary>
        [SerializeField]
        [Tooltip("Determines the number of surperior agents which live on to the next generation")]
        uint eliteCount = 3;

        /// <summary>
        /// If checked the group will evolve when all agents aren't alive
        /// </summary>
        [SerializeField]
        [Tooltip("If checked the group will evolve when all agents aren't alive")]
        bool evolveOnExtinction = true;

        /// <summary>
        /// If checked the group will begin evolving when the monobehaviour function 'Start' is called
        /// </summary>
        [SerializeField]
        [Tooltip("If checked the group will begin evolving when the monobehaviour function 'Start' is called")]
        bool beginOnStart = true;

        /// <summary>
        /// Used for adding agents to the heirarchy. \nIt is still possible to add agents manually, but this might make things easier
        /// </summary>
        [Header("Agent Configuration")]
        [SerializeField]
        [Tooltip("Used for adding agents to the heirarchy. \nIt is still possible to add agents manually, but this might make things easier")]
        GameObject agentPrefab;

        /// <summary>
        /// The amount of values each agent's genome stores
        /// </summary>
        [Tooltip("The amount of values each agent's genome stores")]
        public uint genomeSize = 1;

        /// <summary>
        /// Used to save custom DNA types
        /// </summary>
        [SerializeField]
        [Tooltip("Used to save custom DNA types")]
        DNA agentDNAType;

        /// <summary>
        /// Set this to the total score you would like each agent to get by the time they reach their goal
        /// </summary>
        [SerializeField]
        [Tooltip("Set this to the total score you would like each agent to get by the time they reach their goal")]
        public int rewardThreshold = 0;

        /// <summary>
        /// The greater the importance the more an agent favours higher reward values, the trade off is lower reward values may not be worth enough
        /// </summary>
        [SerializeField]
        [Tooltip("The greater the importance the more an agent favours higher reward values, the trade off is lower reward values may not be worth enough")]
        [Range(1, 10)]
        public int higherRewardImportance = 1;

        [Space(5)]
        [SerializeField]
        TimerSettings timerSettings;

        /// <summary>
        /// All the groups active agents
        /// </summary>
        [Tooltip("All the groups active agents")]
        public EvolutionAgent[] agents;

        /// <summary>
        /// The algorithm used to evolve agents
        /// </summary>
        EvoGeneticAlgorithm geneticAlgorithm;

        /// <summary>
        /// The random object used by the algorithm
        /// </summary>
        Random random;

        IEnumerator Start()
        {
            timerSettings.timer = timerSettings.timerMax;

            if (GeneticTime.instance == null && timerSettings.timerType == TimerSettings.TimerType.GeneticTimer)
                gameObject.AddComponent<GeneticTime>();

            random = new Random();

            if (beginOnStart)
            {
                yield return new WaitForEndOfFrame();
                StartEvolving();
            }
        }

        void LateUpdate()
        {
            if (timerSettings.useTimer)
            {
                switch (timerSettings.timerType)
                {
                    case TimerSettings.TimerType.UnityEngine:
                        Time.timeScale = timerSettings.timeScale;
                        timerSettings.timer -= Time.deltaTime;
                        break;
                    case TimerSettings.TimerType.GeneticTimer:
                        GeneticTime.timeScale = timerSettings.timeScale;
                        timerSettings.timer -= GeneticTime.deltaTime;
                        break;
                }
            }

            if (agents.Length <= 0)
                return;

            if ((evolveOnExtinction && agents.All(a => !a.IsAlive)) || (timerSettings.timer <= 0.0f && timerSettings.useTimer))
            {
                timerSettings.timer = timerSettings.timerMax;
                EvolveGeneration();
            }
        }


        /// <summary>
        /// Call to start evolving the agents
        /// </summary>
        public void StartEvolving()
        {
            LoadAgents();

            InitAgents();

            List<Genome> genomes = agents.Select(a => a.DNA).ToList();

            geneticAlgorithm = new EvoGeneticAlgorithm(genomes, (int)genomeSize, random, this, mutationRate, (int)eliteCount);
        }


        /// <summary>
        /// Intitalises all loaded agents
        /// </summary>
        public void InitAgents()
        {
            foreach (EvolutionAgent agent in agents)
            {
                agent.Init((int)genomeSize, random, agentDNAType, this);
            }
        }


        /// <summary>
        /// Call to move to the next generation
        /// </summary>
        public void EvolveGeneration()
        {
            if (geneticAlgorithm == null)
                return;

            geneticAlgorithm.NewGeneration();

            foreach (var agent in agents)
                agent.ResetAgent();
        }

        /// <summary>
        /// Calculates an agents fitness from their <paramref name="genome"/> and then weights it using the rewards and penalties
        /// </summary>
        /// <param name="genome"></param>
        /// <returns>the final value after calculating fitness and weighting it using rewards and penalties</returns>
        public float EvolutionFitnessFunction(Genome genome)
        {
            EvolutionAgent agent = genome.agent;

            float value = (float)fitnessFunction.GetType()
                .GetMethod("GetValue")
                .Invoke(fitnessFunction, new object[] { agent });

            value = agent.CalculateRewardPenalties(value, rewardThreshold, higherRewardImportance);

            return value;
        }

        /// <summary>
        /// Gets a random value using the <see cref="valueGenerator"/>
        /// </summary>
        /// <returns>returns a random value from the provided <see cref="valueGenerator"/></returns>
        public object GetEvolutionRandomValue()
        {
            return valueGenerator.GetType().GetMethod("GetValue").Invoke(valueGenerator, null);
        }

        /// <summary>
        /// Gets all the child agents and adds them to <see cref="agents"/>, then resets all the agents
        /// </summary>
        public void LoadAgents()
        {
            agents = GetComponentsInChildren<EvolutionAgent>();
            foreach (EvolutionAgent agent in agents)
                agent.ResetAgent();
        }

        /// <summary>
        /// Resets <see cref="agents"/> to an empty array
        /// </summary>
        public void ClearAgents()
        {
            agents = new EvolutionAgent[0];
        }

        /// <summary>
        /// Adds the given <paramref name="agent"/> to the <see cref="geneticAlgorithm"/>
        /// </summary>
        /// <param name="agent">The agent to be addedd</param>
        /// <exception cref="Exception"></exception>
        internal void AddAgent(EvolutionAgent agent)
        {
            if(agents.Contains(agent))
                throw new Exception("The agent you are trying to add is already apart of the group");

            agents = agents.Concat(new EvolutionAgent[] { agent }).ToArray();
            geneticAlgorithm.Population.Add(agent.DNA);
        }
        
        /// <summary>
        /// Removes the given <paramref name="agent"/> from the <see cref="geneticAlgorithm"/>
        /// </summary>
        /// <param name="agent">The agent to be removed</param>
        /// <exception cref="Exception"></exception>
        internal void RemoveAgent(EvolutionAgent agent)
        {
            if(!agents.Contains(agent))
                throw new Exception("The agent you are trying to remove is not already apart of the group");

            agents = agents.Except(new EvolutionAgent[] { agent }).ToArray();
            geneticAlgorithm.Population.Remove(agent.DNA);
        }

        /// <summary>
        /// Gets the generation of the geneticAlgorithm
        /// </summary>
        /// <returns><see cref="geneticAlgorithm.Generation"/></returns>
        public int GetGeneration()
        {
            return geneticAlgorithm != null ? geneticAlgorithm.Generation : 0;
        }

        /// <summary>
        /// Gets the best fitness from the geneticAlgorithm
        /// </summary>
        /// <returns><see cref="geneticAlgorithm.BestFitness"/></returns>
        public float GetBestFitness()
        {
            return geneticAlgorithm != null ? geneticAlgorithm.BestFitness : 0;
        }

        /// <summary>
        /// Auto calculates the recommended mutation rate
        /// </summary>
        /// <returns>1.0f / <see cref="agents.Length"/></returns>
        public float CalculateMutationRate()
        {
            return 1.0f / agents.Length;
        }

        /// <summary>
        /// <paramref name="mutationRate"/> is assigned by the result from <see cref="CalculateMutationRate()"/>
        /// </summary>
        public void AssignMutationRateToCalculatedRate()
        {
            mutationRate = CalculateMutationRate();
        }

        /// <summary>
        /// Instantiates a number of agents and childs them to this object.
        /// </summary>
        /// <param name="addAgentCount">The amount of agents to instantiate</param>
        public void InstantiateNewAgents(int addAgentCount)
        {
            for (int i = 0; i < addAgentCount; i++)
            {
                Instantiate(agentPrefab, transform);
            }
        }
    }
}
