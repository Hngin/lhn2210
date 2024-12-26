using UnityEngine;

public class HandProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public int damage = 10;
    private Vector3 targetPosition;

    private void Start()
    {
        // Destroy projectile after lifetime if it doesn't hit player
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Di chuyển về phía target nếu được thiết lập
        if (targetPosition != null)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    // Hàm Initialize để thiết lập vị trí target của projectile
    public void Initialize(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho player
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
