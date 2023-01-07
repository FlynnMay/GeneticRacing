using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    /// <summary>
    /// Reference to the scene handlers singleton instance
    /// </summary>
    public static SceneHandler Instance { get; private set; }

    /// <summary>
    /// If true the next scene loaded/unloaded will be the last one added to the <paramref name="stack"></paramref>, otherwise it loads from the bottom
    /// </summary>
    [Header("Config")]
    [Tooltip("If true the next scene loaded/unloaded will be the last one added to the stack, otherwise it loads from the bottom")]
    public bool loadTopFirst = false;

    /// <summary>
    /// The current active process waiting to start when a transition begins
    /// </summary>
    List<ITransitionProcess> transitionProcesses = new List<ITransitionProcess>();

    /// <summary>
    /// The processes to be removed once the current transition is complete.
    /// </summary>
    List<ITransitionProcess> transitionProcessesToRemove = new List<ITransitionProcess>();

    /// <summary>
    /// Keeps track of all current scenes which need to be either loaded or unloaded
    /// </summary>
    List<SceneLoadOptions> stack = new List<SceneLoadOptions>();

    /// <summary>
    /// All currently loaded scenes
    /// </summary>
    List<SceneReference> loaded = new List<SceneReference>();

    /// <summary>
    /// Sets up the singleton if one doesn't already exist, also sets <paramref name="timescale"></paramref> to 1.0f to unpause the game.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            return;
        }
        stack = new List<SceneLoadOptions>();
        Time.timeScale = 1f;
    }


    /// <summary>
    /// Loads the menu scene specified through the scene container found in the resources folder, uses transition processs
    /// </summary>
    public void LoadMenuScene()
    {
        LoadScene(SceneContainer.MainScene);

    }

    /// <summary>
    /// Quits the game using transition processes.
    /// </summary>
    public void QuitGame(bool ignoreTransition = true)
    {
        if (ignoreTransition)
            Application.Quit();
        else
            StartCoroutine(QuitGameRoutine());
    }

    /// <summary>
    /// Load the specified scene by using the build index of the scene (found in build settings). 
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene</param>
    /// <param name="delay">The length in seconds for how long a scene should take to load</param>
    /// <param name="mode">Used to determine if a scene should load additively or override the current loaded scenes</param>
    /// <param name="useProcessors">If False, the ITransitionProcess's in <paramref name="transitionProcesses"></paramref> will be ignored and wont be triggered</param>
    /// <param name="sceneLoadAction">Custom event for once the scene has begun loading, gives access to the AsyncOperation used for async loading</param>
    /// <returns>A Guid which can be used as a reference to unload the scene</returns>
    public Guid LoadScene(int sceneIndex, float delay = 0.0f, LoadSceneMode mode = LoadSceneMode.Single, bool useProcessors = true, Action<AsyncOperation> sceneLoadAction = null)
    {
        // Add scene to stack
        SceneReference scene = new SceneReference(type: SceneReferenceType.Index, index: sceneIndex, delay: delay, mode: mode, useProcessors: useProcessors, OnLoadStart: sceneLoadAction);
        AddToStack(new SceneLoadOptions(scene, true));
        return scene.id;
    }

    /// <summary>
    /// Load the specified scene by using the name of the scene (found in build settings). 
    /// </summary>
    /// <param name="sceneName">The name of scene object added to the build settings</param>
    /// <param name="delay">The length in seconds for how long a scene should take to load</param>
    /// <param name="mode">Used to determine if a scene should load additively or override the current loaded scenes</param>
    /// <param name="useProcessors">If False, the ITransitionProcess's in <paramref name="transitionProcesses"></paramref> will be ignored and wont be triggered</param>
    /// <param name="sceneLoadAction">Custom event for once the scene has begun loading, gives access to the AsyncOperation used for async loading</param>
    /// <returns>A Guid which can be used as a reference to unload the scene</returns>
    public Guid LoadScene(string sceneName, float delay = 0.0f, LoadSceneMode mode = LoadSceneMode.Single, bool useProcessors = true, Action<AsyncOperation> sceneLoadAction = null)
    {
        // Add scene to stack
        SceneReference scene = new SceneReference(type: SceneReferenceType.String, name: sceneName, delay: delay, mode: mode, useProcessors: useProcessors, OnLoadStart: sceneLoadAction);
        AddToStack(new SceneLoadOptions(scene, true));
        return scene.id;
    }

    /// <summary>
    /// adds a scene to unload to the stack, by searching for its guid in <paramref name="loaded"/> scenes list.
    /// </summary>
    /// <param name="id">The id provided after calling either of the load scene functions</param>
    public void DropLoaded(Guid id)
    {
        if (loaded.Count <= 0)
            return;

        SceneReference scene = loaded.First(s => s.id == id);
        AddToStack(new SceneLoadOptions(scene, false));
    }

    /// <summary>
    /// Runs the next operation in the stack from either the top or bottom depending on <paramref name="loadTopFirst"></paramref>
    /// </summary>
    void RunStack()
    {
        if (loadTopFirst)
            RunStackTop();
        else
            RunStackBottom();
    }

    /// <summary>
    /// Eithe loads or unloads the scene at the bottom of the <paramref name="stack"></paramref>
    /// </summary>
    public void RunStackBottom()
    {
        SceneLoadOptions sceneLoadOptions = stack.First();
        bool load = sceneLoadOptions.load;
        if (load)
            StartScene(sceneLoadOptions.scene, () => { TransitionEnded(sceneLoadOptions); });
        else
            RemoveScene(sceneLoadOptions.scene, () => { TransitionEnded(sceneLoadOptions); });
    }

    /// <summary>
    /// Eithe loads or unloads the scene at the top of the <paramref name="stack"></paramref>
    /// </summary>
    public void RunStackTop()
    {
        SceneLoadOptions sceneLoadOptions = stack.Last();
        bool load = sceneLoadOptions.load;
        if (load)
            StartScene(sceneLoadOptions.scene, () => { TransitionEnded(sceneLoadOptions); });
        else
            RemoveScene(sceneLoadOptions.scene, () => { TransitionEnded(sceneLoadOptions); });
    }

    /// <summary>
    /// Loads the given scene checking the <paramref name="scene"/> for <paramref name="useProcessors"/> to determine whether or not to trigger the processes in <paramref name="transitiionProcesses"/>
    /// </summary>
    /// <param name="scene">The scene reference to be loaded</param>
    /// <param name="completeAction">The action to be triggeres once the scene is loaded</param>
    private void StartScene(SceneReference scene, Action completeAction)
    {
        if (scene.useProcessors)
            StartCoroutine(LoadSceneWithProcessesRoutine(scene, completeAction));
        else
            LoadSceneWithoutProcesses(scene, completeAction);
    }

    /// <summary>
    /// Unloads the <paramref name="scene"/>, once unloadedd removes it self from <paramref name="loaded"/> and invokes the <paramref name="completeAction"/>
    /// </summary>
    /// <param name="scene">The scene referenct to be unloaded</param>
    /// <param name="completeAction">the action to be triggered once the scene is unloaded</param>
    private void RemoveScene(SceneReference scene, Action completeAction)
    {
        AsyncOperation op = UnloadSceneFromReference(scene);
        op.completed += _ =>
        {
            completeAction?.Invoke();
            loaded.Remove(scene);
        };
    }

    // Stack functions
    /// <summary>
    /// Adds <paramref name="sceneLoadOptions"/> to the <paramref name="stack"></paramref>, if the <paramref name="stack"></paramref> was empty and now isn't it begins running the <paramref name="stack"></paramref>
    /// </summary>
    /// <param name="sceneLoadOptions"></param>
    public void AddToStack(SceneLoadOptions sceneLoadOptions)
    {
        bool wasEmpty = stack.Count == 0;
        stack.Add(sceneLoadOptions);

        if (wasEmpty)
            RunStack();
    }

    // Stack functions
    /// <summary>
    /// Removes <paramref name="sceneLoadOptions"/> from the <paramref name="stack"></paramref>, if the <paramref name="stack"></paramref> was empty and now isn't it begins running the <paramref name="stack"></paramref>.
    /// Clears loaded if the <paramref name="mode"></paramref> in <paramref name="sceneLoadOptions.scene"/> is <paramref name="Single"/>.
    /// </summary>
    /// <param name="sceneLoadOptions"></param>
    private void RemoveStack(SceneLoadOptions sceneLoadOptions)
    {
        stack.Remove(sceneLoadOptions);

        if (sceneLoadOptions.scene.mode == LoadSceneMode.Single)
            loaded.Clear();

        if (stack.Count > 0)
            RunStack();
    }

    // Getters for stack objects
    /// <summary>
    /// Gets the first instance of the <paramref name="id"/> in the <paramref name="stack"/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The first instance of the <paramref name="id"/> in the <paramref name="stack"/> </returns>
    SceneLoadOptions GetLoadOptionsFromStack(Guid id)
    {
        return stack.First(so => so.scene.id == id);
    }

    /// <summary>
    /// Gets the first instance of the <paramref name="scene"/>s id in the <paramref name="stack"/>
    /// </summary>
    /// <param name="scene"></param>
    /// <returns>The first instance of the <paramref name="scene"/>s id in the <paramref name="stack"/></returns>
    SceneLoadOptions GetLoadOptionsFromStack(SceneReference scene)
    {
        return GetLoadOptionsFromStack(scene.id);
    }

    // Async + Coroutines
    /// <summary>
    /// Handles loading the <paramref name="scene"/> by checking it's <paramref name="type"/>
    /// </summary>
    /// <param name="scene">The scene reference to begin loading</param>
    /// <returns>The <paramref name="AsyncOperation"/> given by the <paramref name="LoadSceneAsync"/> function in <paramref name="SceneManager"/></returns>
    private AsyncOperation LoadSceneFromReference(SceneReference scene)
    {
        AsyncOperation op = null;

        switch (scene.type)
        {
            case SceneReferenceType.String:
                op = SceneManager.LoadSceneAsync(scene.name, scene.mode);
                break;
            case SceneReferenceType.Index:
                op = SceneManager.LoadSceneAsync(scene.index, scene.mode);
                break;
            default:
                break;
        }

        return op;
    }

    /// <summary>
    /// Handles unloading the <paramref name="scene"/> by checking it's <paramref name="type"/>
    /// </summary>
    /// <param name="scene">The scene reference to begin unloading</param>
    /// <returns>The <paramref name="AsyncOperation"/> given by the <paramref name="UnloadSceneAsync"/> function in <paramref name="SceneManager"/></returns>
    private AsyncOperation UnloadSceneFromReference(SceneReference scene)
    {
        AsyncOperation op = null;

        switch (scene.type)
        {
            case SceneReferenceType.String:
                op = SceneManager.UnloadSceneAsync(scene.name);
                break;
            case SceneReferenceType.Index:
                op = SceneManager.UnloadSceneAsync(scene.index);
                break;
            default:
                break;
        }

        return op;
    }

    /// <summary>
    /// A Coroutine which handles loading the <paramref name="scene"/> with the process in <paramref name="transitionProcesses"/>
    /// </summary>
    /// <param name="scene">The scene to be loaded</param>
    /// <param name="completeAction">The action to be invoked once the scene is loaded</param>
    /// <returns>A Coroutine</returns>
    IEnumerator LoadSceneWithProcessesRoutine(SceneReference scene, Action completeAction)
    {
        if (scene.delay > 0.0f)
            yield return new WaitForSeconds(scene.delay);

        // Tell all processes to begin
        StartProcesses(scene);

        // Asynchronously load the scene
        AsyncOperation op = LoadSceneFromReference(scene);
        scene.OnLoadStart?.Invoke(op);

        //Ensure scene activation is disabled
        op.allowSceneActivation = false;

        // Don't continue until process is complete
        bool flag = false;
        while (!AllProcessesComplete(scene))
        {
            if (op.progress >= 0.9f && !flag)
            {
                SceneLoadCompleted(scene);
                flag = true;
            }
            yield return null;
        }

        // Let all processes know they are complete
        EndProcesses(scene);

        // Enable scene activation now processes are complete
        op.allowSceneActivation = true;

        completeAction?.Invoke();

        loaded.Add(scene);
    }

    /// <summary>
    /// Loads the <paramref name="scene"/> without the use of any of the <paramref name="transitionProcesses"/>
    /// </summary>
    /// <param name="scene">The scene to be loaded</param>
    /// <param name="completeAction">The action to be invoked once the scene is loaded</param>
    void LoadSceneWithoutProcesses(SceneReference scene, Action completeAction)
    {
        // Asynchronously load the scene
        AsyncOperation op = LoadSceneFromReference(scene);

        scene.OnLoadStart?.Invoke(op);

        completeAction?.Invoke();

        loaded.Add(scene);
    }

    /// <summary>
    /// A Coroutine to quit the game with <paramref name="transitionProcesses"/>
    /// </summary>
    /// <returns>A Coroutine</returns>
    IEnumerator QuitGameRoutine()
    {
        for (int i = 0; i < transitionProcesses.Count; i++)
        {
            ITransitionProcess process = transitionProcesses[i];
            process.OnTransitionStarted();
        }

        while (!AllProcessesComplete())
            yield return null;

        for (int i = 0; i < transitionProcesses.Count; i++)
        {
            ITransitionProcess process = transitionProcesses[i];
            process.OnTransitionEnded();
        }

        Debug.Log("GameQuit");
        Application.Quit();
    }

    // Load w/ processes function
    /// <summary>
    /// Starts the all <paramref name="transitionProcesses"/> which can be activated along side <paramref name="scene"/>.
    /// If no <paramref name="scene"/> is givena all <paramref name="transitionProcesses"/> are started.
    /// </summary>
    /// <param name="scene">The scene being loaded, if no scene exists leave null</param>
    private void StartProcesses(SceneReference? scene = null)
    {
        for (int i = 0; i < transitionProcesses.Count; i++)
        {
            ITransitionProcess process = transitionProcesses[i];

            if (!CanProcessActivate(scene, process))
                continue;

            process.OnTransitionStarted();
        }
    }

    /// <summary>
    /// Informs all the <paramref name="transitionProcesses"/> which that the <paramref name="scene"/> has been loaded and calls their <paramref name="SceneLoadCompleted"/> functions.
    /// If no <paramref name="scene"/> is givena all <paramref name="transitionProcesses"/> are started.
    /// </summary>
    /// <param name="scene">The scene being loaded, if no scene exists leave null</param>
    private void SceneLoadCompleted(SceneReference? scene = null)
    {
        for (int i = 0; i < transitionProcesses.Count; i++)
        {
            ITransitionProcess process = transitionProcesses[i];

            if (CanProcessActivate(scene, process))
                process.SceneLoadCompleted();
        }
    }

    /// <summary>
    /// Once all transitions have ended all processes in <paramref name="transitionProcessesToRemove"/> are removed from <paramref name="transitionProcesses"/>. Also Removes the <paramref name="sceneLoadOptions"/> from the <paramref name="stack"/>
    /// </summary>
    /// <param name="sceneLoadOptions">The scene to remove from the <paramref name="stack"/></param>
    private void TransitionEnded(SceneLoadOptions sceneLoadOptions)
    {
        RemoveStack(sceneLoadOptions);
        transitionProcesses = transitionProcesses.Except(transitionProcessesToRemove).ToList();
        transitionProcessesToRemove.Clear();
    }

    /// <summary>
    /// Calls all the <paramref name="transitionProcesses"/> <paramref name="OnTransitionEnded"/> functions if they can be activate with the scene.
    /// If no <paramref name="scene"/> is given all <paramref name="transitionProcesses"/> are ended.
    /// </summary>
    /// <param name="scene">The scene being loaded, if no scene exists leave null</param>
    private void EndProcesses(SceneReference? scene = null)
    {
        for (int i = 0; i < transitionProcesses.Count; i++)
        {
            ITransitionProcess process = transitionProcesses[i];

            if (!CanProcessActivate(scene, process))
                continue;

            process.OnTransitionEnded();
        }
    }

    /// <summary>
    /// Adds the <paramref name="process"/> to the <paramref name="transitionProcessesToRemove"/> list.
    /// </summary>
    /// <param name="process">The process to be removed</param>
    private void RemoveProcess(ITransitionProcess process)
    {
        transitionProcessesToRemove.Add(process);
    }


    // Static helper function
    /// <summary>
    /// Checks if all processes that can be activated with <paramref name="scene"/> are complete
    /// </summary>
    /// <param name="scene">The scene used to start the processes, If null all processes are checked</param>
    /// <returns>True if all processses are complete</returns>
    private static bool AllProcessesComplete(SceneReference? scene = null)
    {
        return Instance.transitionProcesses.All(p => IsProcessComplete(p, scene));
    }

    /// <summary>
    /// Checks if a <paramref name="process"/> can be activated by the <paramref name="scene"/> and if it is complete
    /// </summary>
    /// <param name="process">The process to check</param>
    /// <param name="scene">The scenw which activated the process</param>
    /// <returns>True if <paramref name="process"/> can be activated by the <paramref name="scene"/> and if it is complete</returns>
    private static bool IsProcessComplete(ITransitionProcess process, SceneReference? scene)
    {
        bool isActiveProcess = CanProcessActivate(scene, process);
        return process.ProcessCompleted() || !isActiveProcess;
    }

    /// <summary>
    /// Checks if the <paramref name="scene"/> can activate the <paramref name="process"/>
    /// </summary>
    /// <param name="scene">The scene to activate the process</param>
    /// <param name="process">The process to check</param>
    /// <returns>True if the provess can activate</returns>
    private static bool CanProcessActivate(SceneReference? scene, ITransitionProcess process)
    {
        return scene.HasValue ? process.GetValidSceneLoadModes().Contains(scene.Value.mode) : true;
    }

    /// <summary>
    /// Adds the <paramref name="process"/> to <paramref name="transitionProcesses"/>
    /// </summary>
    /// <param name="process">The process to add</param>
    public static void AddTransitionProcess(ITransitionProcess process)
    {
        Instance.transitionProcesses.Add(process);
    }

    /// <summary>
    /// Adds the <paramref name="process"/> to the <paramref name="transitionProcessesToRemove"/> list, to be removed once the transitions have completed.
    /// </summary>
    /// <param name="process">The process to remove</param>
    public static void RemoveTransitionProcess(ITransitionProcess process)
    {
        Instance.RemoveProcess(process);
    }
}

public struct SceneLoadOptions
{
    public SceneReference scene;
    public bool load;

    public SceneLoadOptions(SceneReference scene, bool load)
    {
        this.scene = scene;
        this.load = load;
    }
}