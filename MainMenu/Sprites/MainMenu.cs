using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas About; // Reference to the Canvas to show/hide
    public Canvas Str; // Reference to the Canvas to show/hide
    public AudioSource backgroundMusic; // Tham chiếu đến AudioSource phát nhạc nền
    public GameObject continueButton;
    public int defaultHealth = 100; // Giá trị HP mặc định

    void Start()
    {
        About.gameObject.SetActive(false);
        Str.gameObject.SetActive(false);

        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        UpdateContinueButton(); // Kiểm tra trạng thái nút Continue khi khởi động

        // Đăng ký sự kiện với PlayerHealth
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.onPlayerDied.AddListener(HideContinueButton);
            }
        }
    }
    public void HideContinueButton()
    {
        continueButton.SetActive(false); // Ẩn nút Continue
    }

    void Update()
    {
        // Check if the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAbout();
        }
    }

    public void ContinueGame()
    {
        // Tải trạng thái màn chơi
        GameSaveManager.Instance.LoadGame();

        // Kích hoạt Player trước khi bắt đầu coroutine
        StartCoroutine(ResumePlayerState());
    }

    private IEnumerator ResumePlayerState()
    {
        yield return new WaitForSeconds(0.1f); // Đợi màn chơi load xong

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.SetActive(true); // Kích hoạt Player

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.LoadHealth(); // Tải trạng thái máu
            }
        }

        UpdateContinueButton(); // Cập nhật trạng thái nút Continue sau khi tải trạng thái
    }

    // Method to show the Str Canvas
    public void ShowStr()
    {
        // Hiển thị Canvas "Str"
        Str.gameObject.SetActive(true);

        // Dừng phát nhạc nền
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        // Reset trạng thái của Player
        ResetPlayerState();

        // Xóa dữ liệu đã lưu
        GameSaveManager.Instance.ResetAllData(); // Gọi hàm ResetAllData trong GameSaveManager

        UpdateContinueButton(); // Cập nhật trạng thái nút Continue sau khi reset
    }

    private void ResetPlayerState()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Đặt vị trí mặc định
            Vector3 defaultPosition = new Vector3(0, 0, 0); // Thay bằng vị trí mặc định của bạn
            player.transform.position = defaultPosition;

            // Đặt lại máu
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ResetHealth(); // Đặt HP về giá trị mặc định (maxHealth)
            }
        }
    }

    public void UpdateContinueButton()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.currentHealth > 0)
            {
                continueButton.SetActive(true);
                return;
            }
        }

        continueButton.SetActive(false); // Ẩn nếu không có dữ liệu hoặc máu bằng 0
    }



    // Method to show the About Canvas
    public void ShowAbout()
    {
        About.gameObject.SetActive(true);
    }

    // Method to hide the About Canvas
    public void HideAbout()
    {
        About.gameObject.SetActive(false);
    }

    // Method to exit the game
    public void ExitGame()
    {
        Application.Quit();
    }

    // Called from DialogueManager's GoToMainMenu method
    public void HandleDialogueEnd()
    {
        PlayerPrefs.DeleteKey("PlayerCurrentHealth"); // Xóa trạng thái máu
        UpdateContinueButton(); // Cập nhật trạng thái nút Continue
    }

}
