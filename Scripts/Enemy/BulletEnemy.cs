using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float damage = 10f; // Damage dealt to the player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has a specific tag
        if (collision.CompareTag("Obstacle"))
        {
            // Destroy the bullet if it hits an object with specific tags
            Destroy(gameObject);
            return;
        }

        // Check if the bullet hits the player
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Calculate knockback direction (from bullet to player)
            Vector2 knockBackDirection = (collision.transform.position - transform.position).normalized;

            // Apply damage and knockback to the player
            playerHealth.TakeDamage(damage);

            // Destroy bullet after it hits the player
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Destroy the bullet on collision with obstacles
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Destroy the bullet after a certain time to avoid lingering bullets
        Destroy(gameObject, 3f);
    }
}
