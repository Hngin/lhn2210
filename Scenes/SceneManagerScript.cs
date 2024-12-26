using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    private Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSceneState(string sceneName, SceneState state)
    {
        if (sceneStates.ContainsKey(sceneName))
        {
            sceneStates[sceneName] = state;
        }
        else
        {
            sceneStates.Add(sceneName, state);
        }
    }

    public SceneState GetSceneState(string sceneName)
    {
        if (sceneStates.ContainsKey(sceneName))
        {
            return sceneStates[sceneName];
        }
        return null;
    }
}

[System.Serializable]
public class SceneState
{
    public Vector3 playerPosition;
    public Dictionary<string, bool> objectStates = new Dictionary<string, bool>();
}
