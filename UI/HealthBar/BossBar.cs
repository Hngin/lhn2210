using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public Slider bossHealthSlider; 
    [SerializeField] private EnemyHealth boss; 

    private void Start()
    {
        if (boss != null)
        {
            bossHealthSlider.gameObject.SetActive(false); // Ẩn thanh máu ban đầu
            boss.onHealthChanged.AddListener(UpdateBossHealthBar);
            UpdateBossHealthBar(boss.currentHealth / boss.maxHealth);
        }
        else
        {
            Debug.LogWarning("Boss is not assigned in the Inspector.");
        }
    }

    // Phương thức cập nhật giá trị của slider boss health
    public void UpdateBossHealthBar(float healthPercent)
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = healthPercent;
        }
    }

    // Phương thức hiển thị/ẩn thanh máu của boss
    public void ToggleBossHealthBar(bool isVisible)
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(isVisible);
        }
        else
        {
            Debug.LogWarning("Boss health slider is missing or destroyed.");
        }
    }
}
