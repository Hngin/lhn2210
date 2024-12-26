using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public float duration = 2f;     // Thời gian laser tồn tại
    public int damage = 20;         // Sát thương của laser
    private LineRenderer lineRenderer;
    private Transform boss;         // Vị trí của boss để xác định điểm bắn laser
    private Transform target;       // Mục tiêu là player
    private Vector3 targetPosition;


    private void Start()
    {
        // Lấy LineRenderer để tạo laser dưới dạng một đường thẳng
        lineRenderer = GetComponent<LineRenderer>();
        boss = transform;  // Điểm bắn laser từ boss
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Gọi hàm khởi tạo để vẽ laser
        StartCoroutine(FireLaser());
    }
    // Hàm Initialize để thiết lập vị trí target của projectile
    public void Initialize(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }

    private IEnumerator FireLaser()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Cập nhật vị trí laser theo hướng của player
            if (target != null)
            {
                Vector3 direction = (target.position - boss.position).normalized;
                lineRenderer.SetPosition(0, boss.position);                 // Điểm bắn laser từ boss
                lineRenderer.SetPosition(1, boss.position + direction * 10f);  // Đầu còn lại hướng về phía player
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);  // Hủy laser sau khi hết thời gian tồn tại
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho player khi va chạm với laser
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
