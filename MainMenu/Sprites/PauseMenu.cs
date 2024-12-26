using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject manualUI;
    public GameObject settingsUI;

    private bool isPaused = false;
    private bool isManualOpen = false;
    private bool isSettingsOpen = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        manualUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isManualOpen)
            {
                CloseManual();
            }
            else if (isSettingsOpen)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        manualUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ContinueGame()
    {
        pauseMenuUI.SetActive(false);
        manualUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenManual()
    {
        pauseMenuUI.SetActive(false);
        manualUI.SetActive(true);
        settingsUI.SetActive(false);
        isManualOpen = true;
    }

    public void CloseManual()
    {
        manualUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        isManualOpen = false;
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        manualUI.SetActive(false);
        isSettingsOpen = true;
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        isSettingsOpen = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Lưu vị trí và đánh dấu dữ liệu đã tồn tại
            GameSaveManager.Instance.SaveGame(player.transform.position);
            PlayerPrefs.SetInt("HasSavedData", 1);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("MainMenu");
    }

}
