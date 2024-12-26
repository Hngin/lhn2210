using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Tốc độ của đạn
    private Vector2 direction; // Hướng bắn của đạn

    // Phương thức để thiết lập hướng cho đạn
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // Chuẩn hóa hướng bắn
        RotateTowardsDirection(); // Quay đạn về phía player
    }

    private void Update()
    {
        // Cập nhật vị trí của đạn dựa trên hướng và tốc độ
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void RotateTowardsDirection()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Tính góc xoay
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Áp dụng góc xoay lên đạn
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với các đối tượng khác và thực hiện hành động
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10); // Giảm HP của player khi va chạm với đạn
            Destroy(gameObject); // Hủy đạn sau khi va chạm
        }

        // Hủy đạn nếu va chạm với tường hoặc các đối tượng khác
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
