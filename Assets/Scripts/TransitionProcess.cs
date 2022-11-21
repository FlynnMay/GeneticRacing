using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ITransitionProcess
{
    public void OnTransitionStarted();
    public void OnTransitionEnded();
    public bool ProcessStarted();
    public bool ProcessCompleted();
    public void SceneLoadCompleted();
    public List<LoadSceneMode> GetValidSceneLoadModes();
}
