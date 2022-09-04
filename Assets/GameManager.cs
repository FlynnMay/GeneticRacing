using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    RaceManager raceManager;
    public static GameManager Instance { get; private set; }
    public static RaceManager RaceManager { get => Instance.raceManager; }

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
}
