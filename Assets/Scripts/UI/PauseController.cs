using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public bool paused = false;
    [SerializeField] Transform pauseContainer;

    private void Awake()
    {
        pauseContainer.gameObject.SetActive(paused);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            SetPause(!paused);
    }

    public void SetPause(bool paused)
    {
        this.paused = paused;
        if (paused)
            Pause();
        else
            Unpause();
    }

    private void Unpause()
    {
        pauseContainer.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void Pause()
    {
        pauseContainer.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
}

