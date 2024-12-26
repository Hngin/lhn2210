using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime1 : MonoBehaviour
{
    public float health = 100f; // Lượng máu của enemy
    public float moveSpeed = 2f; // Tốc độ di chuyển
    public Animator animator; // Animator để quản lý animation

    private void Update()
    {
        // Kiểm tra trạng thái và thực hiện hành động tương ứng
        if (health <= 0)
        {
            Die();
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        // Di chuyển enemy (ví dụ: di chuyển sang trái)
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        animator.SetBool("isMoving", true); // Animation di chuyển
    }

    public void Attack()
    {
        animator.SetTrigger("attack"); // Animation tấn công
        // Thực hiện tấn công ở đây
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        animator.SetTrigger("hit"); // Animation bị tấn công

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("die"); // Animation chết
        // Thực hiện các hành động khác khi chết, ví dụ: hủy đối tượng sau một khoảng thời gian
        Destroy(gameObject, 2f); // Hủy đối tượng sau 2 giây
    }

    private void OnDisable()
    {
        animator.SetBool("isMoving", false); // Ngừng animation di chuyển khi không còn hoạt động
    }
}