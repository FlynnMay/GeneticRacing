using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
namespace Evo
{
    /// <summary>
    /// The Evolution Agent holds a "Genome" which is used in the gentic algorithm, 
    /// Adding this object as a child of a EvolutionGroup will automatically place it in the Genetic Algorithm's Population
    /// </summary>
    [Serializable]
    public class EvolutionAgent : MonoBehaviour
    {
        /// <summary>
        /// This event is called when agent is reset for a new generation
        /// </summary>
        [HideInInspector]
        public UnityEvent onResetEvent;

        /// <summary>
        /// Determines the output and input types of an agent's DNA, This is used when calling the ExportDNA function 
        /// </summary>
        [HideInInspector] public DNA DNAType;

        [Tooltip("Overwrites default DNA")]
        public DNA defaultDNA = null;

        /// <summary>
        /// The object which holds all the Genes and is used inside of the genetic algorithm
        /// </summary>
        public Genome DNA { get; set; }

        /// <summary>
        /// If True this agent was one of the best fit agents from the last population
        /// </summary>
        public bool IsElite { get { return DNA.IsElite; } }

        /// <summary>
        /// If True this agent had the highest fitness int the last population
        /// </summary>
        public bool IsKing { get { return DNA.IsKing; } }

        /// <summary>
        /// Set to false when the agent is finished it's task
        /// </summary>
        public bool IsAlive { get; set; } = true;

        /// <summary>
        /// The Evolution Group the agent is apart of
        /// </summary>
        [HideInInspector] public EvolutionGroup group;

        /// <summary>
        /// This the value which is updated when 
        /// </summary>
        public int Score { get; private set; }
        
        /// <summary>
        /// If unchecked the agent will run with out a EvolutionGroup using the Default DNA.
        /// </summary>
        [SerializeField]
        bool training = true;

        private void Awake()
        {
            if (training) 
                return;

            if (defaultDNA == null)
                throw new Exception("Default DNA cannot be null outside of training.");

            DNA = new Genome(this, 0);
            ApplyDefaultDNA();
        }

        /// <summary>
        /// Called in EvolutionGroup to initialise the agent
        /// </summary>
        /// <param name="size"></param>
        /// <param name="random"></param>
        /// <param name="_DNAType"></param>
        /// <param name="_group"></param>
        public void Init(int size, System.Random random, DNA _DNAType, EvolutionGroup _group)
        {
            if (!training)
                throw new Exception("Trying to initialise a non-training agent. Only training agents can be initialised through an EvolutionGroup");

            group = _group;
            DNA = new Genome(size, random, _group, this);
            DNAType = _DNAType;

            if (defaultDNA != null)
                ApplyDefaultDNA();
        }

        /// <summary>
        /// Called in Evolve Generation. Resets generation specific information and tiggers the reset event.
        /// </summary>
        public void ResetAgent()
        {
            Score = 0;
            IsAlive = true;
            onResetEvent?.Invoke();
        }
#if UNITY_EDITOR
        /// <summary>
        /// Saves DNA to a ScriptableObject
        /// </summary>
        public void ExportDNA()
        {
            Type type = DNAType.GetType();
            ScriptableObject exportObject = ScriptableObject.CreateInstance(type);

            type.GetMethod("SetGenesFromObject", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(exportObject, new object[] { DNA.Genes });

            string path = Tools.FilePathWizard.GetPath(Tools.FilePathWizard.agentsKey) + name + ".asset";
            AssetDatabase.CreateAsset(exportObject, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = exportObject;
        }
#endif

        /// <summary>
        /// Decreases score by an amount
        /// </summary>
        /// <param name="amount"></param>
        public void Penalise(int amount = 1)
        {
            Score -= amount;
        }

        /// <summary>
        /// Increase score by an amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reward(int amount = 1)
        {
            Score += amount;
        }

        /// <summary>
        /// Called at the end of the fitness function
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rewardThreshold"></param>
        /// <returns>the input value weighted by the score and rewardThreshold</returns>
        public float CalculateRewardPenalties(float value, float rewardThreshold, int rewardImportance)
        {
            if (rewardThreshold == 0)
                return value;

            float scoreTemp = Mathf.Clamp(Score, int.MinValue, rewardThreshold);
            int modifier = scoreTemp >= 0 ? 1 : -1;
            float num = value * Mathf.Pow(Mathf.Abs(scoreTemp) / rewardThreshold, rewardImportance) * modifier;

            return (Mathf.Clamp(num, -1, 1) + 1) / 2;
        }

        /// <summary>
        /// Sets the default DNA then updates the the genes of the current agents DNA
        /// </summary>
        /// <param name="DNA"></param>
        public void SetAndApplyDNA(DNA DNA)
        {
            SetDefaultDNA(DNA);
            ApplyDefaultDNA();
        }

        /// <summary>
        /// Sets the defaultDNA, apply using ApplyDefaultDNA
        /// </summary>
        /// <param name="DNA"></param>
        public void SetDefaultDNA(DNA DNA)
        {
            defaultDNA = DNA;
        }

        /// <summary>
        /// Sets the DNA's Genes using the default DNA
        /// </summary>
        public void ApplyDefaultDNA()
        {
            Type type = defaultDNA.GetType();
            FieldInfo info = type.GetField("genes");
            object genes = info.GetValue(defaultDNA);
            DNA.Genes = ((IEnumerable)genes).Cast<object>().ToArray();
        }
    }
}