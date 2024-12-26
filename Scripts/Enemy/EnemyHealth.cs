using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public float knockBackDistance = 2f; // Controls the knockback distance
    public float knockBackSpeed = 10f;   // Controls the speed of knockback
    public float flashDuration = 0.1f;
    public GameObject[] itemDrops; // Danh sách các itemDrop khác nhau
    public int minDropCount = 1;
    public int maxDropCount = 3;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public float immobilizeDuration = 0.5f; // Time enemy is immobilized after knockback
    public GameObject destructionEffect; // Hiệu ứng tan biến
    public UnityEvent<float> onHealthChanged; // Event for health change

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (onHealthChanged == null)
        {
            onHealthChanged = new UnityEvent<float>();
        }

        // Invoke the event with the initial health percentage
        onHealthChanged.Invoke(currentHealth / maxHealth);
    }

    public void TakeDamage(float damage, Vector2 knockBackDir)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp HP between 0 and maxHealth

        // Notify listeners of the health change
        onHealthChanged.Invoke(currentHealth / maxHealth);

        StartCoroutine(Flash());
        knockBackDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        ApplyKnockback(knockBackDir);

        if (currentHealth <= 0)
        {
            Animator animator = GetComponent<Animator>();
            StartCoroutine(HandleDeath(animator));
            DropItems();
        }
    }

    private IEnumerator HandleDeath(Animator animator)
    {
        // Hiển thị hiệu ứng tan biến
        Instantiate(destructionEffect, transform.position, Quaternion.identity);

        // Đợi một chút trước khi destroy object
        yield return new WaitForSeconds(0f);
        Destroy(gameObject);
    }

    private void ApplyKnockback(Vector2 direction)
    {
        direction = direction.normalized; // Normalize to maintain direction without affecting distance
        rb.velocity = direction * knockBackSpeed; // Apply high speed instantly
        StartCoroutine(StopKnockback());
    }

    private IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(knockBackDistance / knockBackSpeed); // Time to travel knockbackDistance
        rb.velocity = Vector2.zero; // Stop the knockback after reaching the target distance
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
    }

    private void DropItems()
    {
        int dropCount = Random.Range(minDropCount, maxDropCount);
        for (int i = 0; i < dropCount; i++)
        {
            // Chọn một item ngẫu nhiên từ danh sách
            GameObject itemDrop = itemDrops[Random.Range(0, itemDrops.Length)];

            // Generate a random position around the enemy within a radius
            Vector2 randomOffset = Random.insideUnitCircle.normalized; // Random direction
            Vector3 dropPosition = transform.position + (Vector3)randomOffset * 0.5f; // Adjust initial spawn radius if needed

            // Instantiate the item at the drop position
            GameObject droppedItem = Instantiate(itemDrop, dropPosition, Quaternion.identity);

            // Apply an outward force to simulate the item flying out
            Rigidbody2D itemRb = droppedItem.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                float launchForce = Random.Range(2f, 5f); // Adjust force as needed
                itemRb.AddForce(randomOffset * launchForce, ForceMode2D.Impulse);
            }
        }
    }
}
