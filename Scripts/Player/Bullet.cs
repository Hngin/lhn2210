using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float attackDamage = 20f; 
    public float knockBackForce = 5f; 
    private Vector3 mousePos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;

    public float lifetime = 3f;

    public AudioSource audioSource; 
    public AudioClip ImpactBow;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;

        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rot);

        StartCoroutine(DestroyAfterLifetime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has a specific tag
        if (collision.CompareTag("Obstacle"))
        {
            // Destroy the bullet if it hits an object with specific tags
            Destroy(gameObject);
            return;
        }

        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            
                PlaySound(ImpactBow); // Phát âm thanh khi di chuyển
            
            Vector2 knockBackDirection = (collision.transform.position - transform.position).normalized;

            enemyHealth.TakeDamage(attackDamage, knockBackDirection * knockBackForce);

            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(knockBackDirection * knockBackForce, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
        }
    }
    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
