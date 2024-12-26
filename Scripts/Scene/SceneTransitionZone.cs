using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionZone : MonoBehaviour
{
    public string sceneToLoad = "S2"; // Tên scene mới sẽ chuyển đến
    public Vector3 spawnPositionInNewScene = Vector3.zero; // Vị trí spawn trong scene mới

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Lưu trạng thái game
            SaveGameState(collision.gameObject);

            // Đảm bảo không phá hủy player khi chuyển scene
            DontDestroyOnLoad(collision.gameObject);

            // Đăng ký callback khi scene được load
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Chuyển đến scene mới
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void SaveGameState(GameObject player)
    {
        // Lưu thông tin game qua GameSaveManager
        GameSaveManager.Instance.SaveGame(player.transform.position);

        // Lưu máu của player thông qua PlayerHealth
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SaveHealth();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Hủy đăng ký sự kiện sau khi xử lý xong
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Phục hồi trạng thái game trong scene mới
        RestoreGameState(scene.name);
    }

    private void RestoreGameState(string sceneName)
    {
        // Lấy đối tượng Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Khôi phục vị trí của player
        if (player != null)
        {
            Vector3 savedPosition = GameSaveManager.Instance.GetSavedPlayerPosition();
            player.transform.position = spawnPositionInNewScene != Vector3.zero ? spawnPositionInNewScene : savedPosition;

            // Khôi phục máu của player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.LoadHealth();
            }
        }

        // Có thể thêm các logic phục hồi trạng thái khác ở đây nếu cần
    }
}
