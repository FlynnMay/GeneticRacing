using System;
using UnityEngine.SceneManagement;

public struct SceneReference
{
    public string name;
    public int index;
    public SceneReferenceType type;
    public LoadSceneMode mode;
    public Guid id;
    public float delay;
    public Action<UnityEngine.AsyncOperation> OnLoadStart;
    public bool useProcessors;

    public SceneReference(SceneReferenceType type, string name = "null", int index = -1, float delay = 0.0f, LoadSceneMode mode = LoadSceneMode.Single, bool useProcessors = true, Action<UnityEngine.AsyncOperation> OnLoadStart = null)
    {
        this.name = name;
        this.index = index;
        this.type = type;
        this.mode = mode;
        this.delay = delay;
        this.OnLoadStart = OnLoadStart;
        this.useProcessors = useProcessors;
        id = Guid.NewGuid();
    }

    public static implicit operator SceneReference(string name) => new SceneReference(SceneReferenceType.String, name: name);
    public static implicit operator SceneReference(int index) => new SceneReference(SceneReferenceType.Index, index: index);
}