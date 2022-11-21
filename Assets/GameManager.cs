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
    public bool IsTraining { get => isTraining; set => isTraining = value; }

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

        if (IsTraining)
            group.StartEvolving();
    }
}
