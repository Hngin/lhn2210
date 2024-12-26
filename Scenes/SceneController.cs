using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private string lastScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            lastScene = SceneManager.GetActiveScene().name;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void ContinueLastScene()
    {
        if (!string.IsNullOrEmpty(lastScene))
        {
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("No last scene to continue.");
        }
    }
}
