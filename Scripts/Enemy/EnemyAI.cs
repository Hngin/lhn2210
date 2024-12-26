using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectRange = 5f;
    public float dashRange = 2f;
    public float dashSpeed = 10f;
    public float delayBetweenDashes = 2f;
    private Transform player;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool isChasing = false;
    private float nextAttackTime = 0f;
    public int damageAmount = 10;
    public float attackCooldown = 1f;
    public float knockBackForce = 5f; // Knockback force applied to enemies

    private Animator animator; // Animator reference

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Initialize Animator

        // Tự động tìm đối tượng Player
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found in the scene! Make sure the player has the tag 'Player'.");
        }

        StartCoroutine(RandomMovement());
    }

    private void Update()
    {
        if (player == null) return; // Kiểm tra nếu player vẫn chưa được tìm thấy

        float playerDistance = Vector2.Distance(transform.position, player.position);
        if (playerDistance <= detectRange && !isDashing)
        {
            isChasing = true;
            MoveTowardsPlayer();
            if (playerDistance <= dashRange)
            {
                StartCoroutine(DashAndAttack());
            }
        }
        else
        {
            isChasing = false;
        }

        // Flip enemy sprite based on movement direction
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(2, 2, 1); // Facing right
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-2, 2, 1); // Facing left
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private IEnumerator DashAndAttack()
    {
        isDashing = true;

        Vector2 direction = (player.position - transform.position).normalized;
        float dashDistance = dashRange;

        animator.SetTrigger("Attack"); // Start the attack animation at the beginning of dash

        while (Vector2.Distance(transform.position, player.position) > 0.5f && dashDistance > 0)
        {
            rb.velocity = direction * dashSpeed;
            yield return null;
            dashDistance -= (direction * dashSpeed * Time.deltaTime).magnitude;
        }

        rb.velocity = Vector2.zero;

        if (Vector2.Distance(transform.position, player.position) <= 0.5f)
        {
            PerformAttack();
        }

        yield return new WaitForSeconds(delayBetweenDashes);
        isDashing = false;
    }

    private void PerformAttack()
    {
        if (Time.time >= nextAttackTime && player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator RandomMovement()
    {
        while (true)
        {
            if (!isDashing && !isChasing)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.velocity = randomDirection * moveSpeed;
                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (Time.time >= nextAttackTime) // Ensure attack cooldown
                {
                    playerHealth.TakeDamage(damageAmount);
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }
}
