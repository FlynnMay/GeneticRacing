using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool isTraining = false;
    [SerializeField] Evo.EvolutionGroup group;
    [SerializeField] GameObject trainingPanel;
    [SerializeField] FollowAllCars followCars;
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
        trainingPanel.SetActive(IsTraining);
        if (IsTraining)
            StartTraining();

        RaceManager.SetupCars();
        followCars.SetupCars();
    }

    private void StartTraining()
    {
        group.InstantiateNewAgents(300);
        group.LoadAgents();
        //group.InitAgents();

        for (int i = 0; i < group.agents.Length; i++)
        {
            Evo.EvolutionAgent agent = group.agents[i];
            agent.SetTraining(true, false);
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

    public void OverrideSpectate()
    {

    }
}
