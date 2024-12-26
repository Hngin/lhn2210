using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip foodSound;
    public AudioClip coinSound;

    [Header("Audio Source")]
    public AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra tag của object va chạm
        if (collision.CompareTag("Food"))
        {
            PlaySound(foodSound);
        }
        else if (collision.CompareTag("Coin"))
        {
            PlaySound(coinSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioSource hoặc AudioClip chưa được gán!");
        }
    }
}
