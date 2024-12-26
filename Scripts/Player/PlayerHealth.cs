using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f; // Máu tối đa của Player
    public float currentHealth;   // Máu hiện tại của Player
    public float invincibilityTime = 1f; // Thời gian bất tử sau khi bị tấn công
    private bool isInvincible = false; // Trạng thái bất tử

    [SerializeField] private AudioClip hurtSound;      // Âm thanh khi bị sát thương
    [SerializeField] private AudioClip deathSound;     // Âm thanh khi chết
    [SerializeField] private Canvas gameOverCanvas;    // Hiển thị Game Over

    private AudioSource audioSource;                  // Thành phần phát âm thanh
    private SpriteRenderer spriteRenderer;            // Để hiển thị hiệu ứng flash
    public UnityEvent<float> onHealthChanged;         // Sự kiện thay đổi thanh máu
    public UnityEvent onPlayerDied; // Sự kiện khi máu bằng 0

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (PlayerPrefs.HasKey("PlayerCurrentHealth"))
        {
            LoadHealth();
        }
        else
        {
            currentHealth = maxHealth;
        }

        gameOverCanvas?.gameObject.SetActive(false); // Tắt canvas game over nếu có

        onHealthChanged?.Invoke(currentHealth / maxHealth); // Cập nhật thanh máu ban đầu
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return; // Nếu đang bất tử, bỏ qua sát thương

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        PlaySound(hurtSound); // Phát âm thanh sát thương
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        StartCoroutine(FlashEffect()); // Hiệu ứng khi bị tấn công

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void Heal(float healAmount)
    {
        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            onHealthChanged?.Invoke(currentHealth / maxHealth); // Cập nhật UI
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player has died!");

        PlaySound(deathSound);

        gameOverCanvas?.gameObject.SetActive(true); // Hiển thị game over nếu có

        if (spriteRenderer != null)
        {
            StartCoroutine(FadeOut()); // Làm mờ nhân vật
        }

        Time.timeScale = 0; // Dừng game

        onPlayerDied?.Invoke(); // Kích hoạt sự kiện khi chết
    }

    private IEnumerator FadeOut()
    {
        if (spriteRenderer == null) yield break;

        Color color = spriteRenderer.color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.05f)
        {
            color.a = alpha;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SaveHealth()
    {
        PlayerPrefs.SetFloat("PlayerCurrentHealth", currentHealth);
        PlayerPrefs.SetFloat("PlayerMaxHealth", maxHealth);
        PlayerPrefs.Save();
    }

    public void LoadHealth()
    {
        if (PlayerPrefs.HasKey("PlayerCurrentHealth") && PlayerPrefs.HasKey("PlayerMaxHealth"))
        {
            currentHealth = PlayerPrefs.GetFloat("PlayerCurrentHealth");
            maxHealth = PlayerPrefs.GetFloat("PlayerMaxHealth");
        }
        else
        {
            currentHealth = maxHealth;
        }

        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth / maxHealth);
        SaveHealth();
    }

    public static void StaticResetHealth()
    {
        float defaultMaxHealth = 100f;
        PlayerPrefs.SetFloat("PlayerMaxHealth", defaultMaxHealth);
        PlayerPrefs.SetFloat("PlayerCurrentHealth", defaultMaxHealth);
        PlayerPrefs.Save();

        Debug.Log("Player health has been reset to default.");
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }
}
