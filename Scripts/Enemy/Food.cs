using UnityEngine;

public class Food : MonoBehaviour
{
    public float healAmount = 10f; // Số lượng HP hồi phục

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Kiểm tra nếu player chưa đầy HP
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    // Hồi phục HP
                    playerHealth.Heal(healAmount);

                    // Phá hủy đối tượng food
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Player is already at full health!");
                }
            }
        }
    }
}
