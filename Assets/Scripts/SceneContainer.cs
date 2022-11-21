using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[CreateAssetMenu(menuName = "Scene Container", fileName = "SceneContainer")]
public class SceneContainer : ScriptableObject
{
    public string mainScene;

    static SceneContainer instance;
    static SceneContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("SceneContainer") as SceneContainer;
            }
            return instance;
        }
    }

    public static string MainScene { get { return Instance.mainScene; } }
}
