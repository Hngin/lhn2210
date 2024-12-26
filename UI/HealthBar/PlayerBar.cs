using UnityEngine;
using UnityEngine.UI;

public class PlayerBar : MonoBehaviour
{
    public Slider playerHealthSlider; // Slider for player health
    public Slider staminaSlider; // Slider for player stamina
    public PlayerHealth playerHealth; // Reference to PlayerHealth
    public PlayerController playerController; // Reference to PlayerController

    private void Start()
    {
        if (playerHealth != null)
        {
            // Đăng ký sự kiện thay đổi HP cho player
            playerHealth.onHealthChanged.AddListener(UpdatePlayerHealthBar);
            UpdatePlayerHealthBar(playerHealth.currentHealth / playerHealth.maxHealth);
        }

        if (playerController != null)
        {
            UpdateStaminaBar(playerController.currentStamina / playerController.maxStamina);
        }
    }

    private void Update()
    {
        if (playerController != null)
        {
            // Cập nhật thanh stamina
            UpdateStaminaBar(playerController.currentStamina / playerController.maxStamina);

            // Cập nhật thanh máu của player
            UpdatePlayerHealthBar(playerHealth.currentHealth / playerHealth.maxHealth);
        }
    }

    // Cập nhật giá trị của slider player health
    private void UpdatePlayerHealthBar(float healthPercent)
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = healthPercent;
        }
    }

    // Cập nhật giá trị của slider stamina
    public void UpdateStaminaBar(float staminaPercent)
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = staminaPercent;
        }
    }
}

