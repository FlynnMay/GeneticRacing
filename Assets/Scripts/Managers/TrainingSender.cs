using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSender : MonoBehaviour
{
    public void SetTraining(bool training)
    {
        PlayerPrefs.SetInt(GRPrefKeys.GRTraining, training ? 1 : 0);
    }
}

public static class GRPrefKeys
{
    public const string GRTraining = "GR.T";
    public const string GRIndicies = "GR.I";
    public const string GRIndiciesCount = "GR.ICount";
}