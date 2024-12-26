using UnityEngine.SceneManagement;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;
    public bool hasResetData = false;
    private const string SwordUpgradeKey = "SwordUpgradeLevel";
    private const string BowUpgradeKey = "BowUpgradeLevel";

    public static void SaveSwordUpgradeLevel(int level)
    {
        PlayerPrefs.SetInt(SwordUpgradeKey, level);
        PlayerPrefs.Save();
    }

    public static int GetSwordUpgradeLevel()
    {
        return PlayerPrefs.GetInt(SwordUpgradeKey, 0);
    }

    public static void SaveBowUpgradeLevel(int level)
    {
        PlayerPrefs.SetInt(BowUpgradeKey, level);
        PlayerPrefs.Save();
    }

    public static int GetBowUpgradeLevel()
    {
        return PlayerPrefs.GetInt(BowUpgradeKey, 0);
    }

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

    public void SaveGame(Vector3 playerPosition)
    {
        PlayerPrefs.SetString("CurrentScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SaveHealth();
        }

        if (CoinManager.instance != null)
        {
            CoinManager.instance.SaveCoins();
        }
        Sword sword = FindObjectOfType<Sword>();
        if (sword != null)
        {
            sword.SaveSwordData();
        }
        PlayerAimWeapon playerAimWeapon = FindObjectOfType<PlayerAimWeapon>();
        if (playerAimWeapon != null)
        {
            playerAimWeapon.SavePlayerAimWeaponData();
        }
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("CurrentScene"))
        {
            string sceneToLoad = PlayerPrefs.GetString("CurrentScene");
            SceneManager.LoadScene(sceneToLoad);

            SceneManager.sceneLoaded += RestorePlayerState;
        }
    }

    private void RestorePlayerState(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= RestorePlayerState;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = GetSavedPlayerPosition();

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.LoadHealth();
            }
        }

        if (CoinManager.instance != null)
        {
            CoinManager.instance.LoadCoins();
        }

        Sword sword = FindObjectOfType<Sword>();
        if (sword != null)
        {
            sword.LoadSwordData();
        }

        PlayerAimWeapon playerAimWeapon = FindObjectOfType<PlayerAimWeapon>();
        if (playerAimWeapon != null)
        {
            playerAimWeapon.LoadPlayerAimWeaponData();
        }
    }

    public Vector3 GetSavedPlayerPosition()
    {
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            return new Vector3(x, y, z);
        }
        return Vector3.zero;
    }

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerHealth.StaticResetHealth();

        if (CoinManager.instance != null)
        {
            CoinManager.instance.ResetCoins();
        }

        Sword.ResetSwordData();
        PlayerAimWeapon.ResetPlayerAimWeaponData();

        Debug.Log("All saved data has been reset, including player health, coins, and upgrades.");
        hasResetData = true;
    }
}
