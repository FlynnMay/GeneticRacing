using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static RaceManager RaceManager { get=> Instance.raceManager; }
    RaceManager raceManager;

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
