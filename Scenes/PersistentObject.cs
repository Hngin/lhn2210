using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObject : MonoBehaviour
{
    private static PersistentObject instance;

    private void Awake()
    {

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Đăng ký sự kiện sceneLoaded để kiểm tra khi scene được nạp
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi đối tượng này bị phá hủy
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra nếu scene hiện tại là MainMenu
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
            instance = null;
        }
    }
}