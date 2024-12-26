using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradeCanvas;
    public Button upgradeSwordButton;
    public Button upgradeBowButton;
    public TMP_Text upgradeMessage;
    public int swordUpgradeCost = 10;
    public int bowUpgradeCost = 10;

    private int swordUpgradeLevel = 0;
    private int bowUpgradeLevel = 0;

    private Sword sword;
    private PlayerAimWeapon playerAimWeapon;
    private CoinManager coinManager;

    private void Start()
    {
        sword = FindObjectOfType<Sword>();
        playerAimWeapon = FindObjectOfType<PlayerAimWeapon>();
        coinManager = CoinManager.instance;

        swordUpgradeLevel = GameSaveManager.GetSwordUpgradeLevel();
        bowUpgradeLevel = GameSaveManager.GetBowUpgradeLevel();

        if (swordUpgradeLevel >= 3)
        {
            upgradeSwordButton.gameObject.SetActive(false);
        }

        if (bowUpgradeLevel >= 3)
        {
            upgradeBowButton.gameObject.SetActive(false);
        }

        upgradeCanvas.SetActive(false);

        upgradeSwordButton.onClick.AddListener(UpgradeSword);
        upgradeBowButton.onClick.AddListener(UpgradeBow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (upgradeCanvas.activeSelf)
            {
                Time.timeScale = 1f;
                upgradeCanvas.SetActive(false);
            }
            else
            {
                Time.timeScale = 0f;
                upgradeCanvas.SetActive(true);
            }
        }
    }

        private void UpgradeSword()
        {
            if (swordUpgradeLevel < 3 && coinManager.GetTotalCoins() >= swordUpgradeCost)
            {
                coinManager.AddCoins(-swordUpgradeCost);
                sword.attackDamage += 5;
                swordUpgradeLevel++;
                sword.SaveSwordData();
                upgradeMessage.text = "Nâng cấp kiếm thành công!";
                StartCoroutine(HideUpgradeMessage());
            }
            else
            {
                upgradeMessage.text = "Không đủ vàng để nâng cấp!";
                StartCoroutine(HideUpgradeMessage());
            }

            if (swordUpgradeLevel >= 3)
            {
                upgradeSwordButton.gameObject.SetActive(false);
            }

            GameSaveManager.SaveSwordUpgradeLevel(swordUpgradeLevel);
        }

        private void UpgradeBow()
        {
            if (bowUpgradeLevel < 3 && coinManager.GetTotalCoins() >= bowUpgradeCost)
            {
                coinManager.AddCoins(-bowUpgradeCost);

                if (bowUpgradeLevel < 2)
                {
                    playerAimWeapon.parallelShotLevel++;
                }
                else if (bowUpgradeLevel == 2)
                {
                    playerAimWeapon.maxShots = 10;
                }

                bowUpgradeLevel++;
                playerAimWeapon.SavePlayerAimWeaponData();
                upgradeMessage.text = "Nâng cấp cung thành công!";
                StartCoroutine(HideUpgradeMessage());
            }
            else
            {
                upgradeMessage.text = "Không đủ vàng để nâng cấp!";
                StartCoroutine(HideUpgradeMessage());
            }

            if (bowUpgradeLevel >= 3)
            {
                upgradeBowButton.gameObject.SetActive(false);
            }

            GameSaveManager.SaveBowUpgradeLevel(bowUpgradeLevel);
        }

        private IEnumerator HideUpgradeMessage()
        {
            yield return new WaitForSecondsRealtime(1f);
            upgradeMessage.text = "";
        }
    }
