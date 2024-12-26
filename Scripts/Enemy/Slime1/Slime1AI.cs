using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime1AI : MonoBehaviour
{
    public float moveSpeed = 2f; // Tốc độ di chuyển
    public float detectionRange = 5f; // Khoảng cách phát hiện người chơi
    public float attackRange = 2f; // Khoảng cách tấn công
    public float health = 50f; // Lượng máu của slime
    public Transform player; // Tham chiếu đến người chơi
    public Animator animator; // Animator để quản lý animation

    private Vector2 targetPosition;
    private float changeDirectionTime = 2f; // Thời gian thay đổi hướng di chuyển
    private float timer;

    private void Start()
    {
        SetNewRandomTargetPosition();
        timer = changeDirectionTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        // Kiểm tra khoảng cách đến người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            if (timer <= 0)
            {
                SetNewRandomTargetPosition();
                timer = changeDirectionTime;
            }

            MoveTowardsTarget();
        }
    }

    private void SetNewRandomTargetPosition()
    {
        // Tạo vị trí ngẫu nhiên trong khoảng cách nhất định
        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(-5f, 5f);
        targetPosition = new Vector2(transform.position.x + randomX, transform.position.y + randomY);
    }

    private void MoveTowardsTarget()
    {
        // Di chuyển đến vị trí mục tiêu
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Cập nhật animation di chuyển
        animator.SetBool("isMoving", true);

        // Kiểm tra nếu đã đến vị trí mục tiêu
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            animator.SetBool("isMoving", false); // Ngừng animation di chuyển
        }
    }

    private void ChasePlayer()
    {
        // Di chuyển về phía người chơi
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        animator.SetBool("isMoving", true); // Animation di chuyển khi đang đuổi theo

        if (Vector2.Distance(transform.position, player.position) < attackRange)
        {
            Attack(); // Tấn công nếu trong phạm vi tấn công
        }
    }

    private void Attack()
    {
        animator.SetTrigger("attack"); // Animation tấn công
        // Thực hiện logic tấn công ở đây (ví dụ: gây sát thương cho người chơi)
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("die"); // Animation chết
        Destroy(gameObject, 2f); // Hủy đối tượng sau 2 giây
    }
}