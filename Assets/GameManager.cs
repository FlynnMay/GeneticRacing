using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool isTraining = false;
    [SerializeField] Evo.EvolutionGroup group;
    RaceManager raceManager;
    public static GameManager Instance { get; private set; }
    public static RaceManager RaceManager { get => Instance.raceManager; }
    public bool IsTraining { get => isTraining || GetShouldTrain(); }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            return;
        }

        raceManager = GetComponentInChildren<RaceManager>();
    }

    private void Start()
    {
        if (IsTraining)
            StartTraining();
    }

    private void StartTraining()
    {
        group.InstantiateNewAgents(300);
        
        for (int i = 0; i < group.agents.Length; i++)
        {
            Evo.EvolutionAgent agent = group.agents[i];
            agent.SetTraining(true);
        }

        group.StartEvolving();
    }

    private bool GetShouldTrain()
    {
        return PlayerPrefs.GetInt(GRPrefKeys.GRTraining) == 1;
    }
    
    private void DeleteKey()
    {
        PlayerPrefs.DeleteKey(GRPrefKeys.GRTraining);
    }
}
