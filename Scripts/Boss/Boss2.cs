using System.Collections;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float detectRange = 10f;
    public float dashRange = 15f;
    public float dashSpeed = 15f;
    public float delayBetweenDashes = 1f;
    public int maxDashCount = 3;
    public int damageAmount = 10;
    public GameObject projectilePrefab;
    public float shootingCooldown = 0.5f;
    public int projectileCount = 8;
    public TrailRenderer myTrailRenderer;

    // Audio clips
    public AudioClip detectionAudio;
    public AudioClip dashAudio;
    public AudioClip shootAudio;

    private Transform player;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool isChasing = false;
    private bool isShooting = false;
    private int dashCount = 0;
    private Animator animator;
    private PlayerHealth playerHealth;
    private BossBar healthBar;
    private AudioSource audioSource;

    private bool playerDetected = false;

    private void Start()
    {
        // Tìm và tham chiếu đối tượng Player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure the player has the tag 'Player'.");
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar = FindObjectOfType<BossBar>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is missing! Please add an AudioSource component to the Boss.");
        }

        // Reset trạng thái boss
        OnEnable();

        StartCoroutine(RandomMovement());
    }


    private void Update()
    {
        if (player == null)
        {
            healthBar.ToggleBossHealthBar(false); // Ẩn thanh máu nếu không tìm thấy Player
            return;
        }

        float playerDistance = Vector2.Distance(transform.position, player.position);

        // Kiểm tra nếu Player nằm trong phạm vi phát hiện
        if (playerDistance <= detectRange && !playerDetected)
        {
            playerDetected = true;
            healthBar.ToggleBossHealthBar(true); // Hiện thanh máu khi phát hiện Player

            // Phát âm thanh phát hiện
            PlaySound(detectionAudio);
        }

        if (playerDetected && !isDashing && !isShooting)
        {
            isChasing = true;
            MoveTowardsPlayer();

            if (dashCount < maxDashCount)
            {
                StartCoroutine(DashAndAttack());
            }
            else if (!isShooting)
            {
                StartCoroutine(RetreatAndShoot());
            }
        }
        else
        {
            isChasing = false;
        }

        // Lật sprite dựa vào hướng di chuyển
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(3, 3, 1);
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-3, 3, 1);
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
        myTrailRenderer.emitting = true;
        dashCount++;

        Vector2 direction = (player.position - transform.position).normalized;
        float dashDistance = dashRange;
        animator.SetTrigger("Attack");

        // Phát âm thanh dash
        PlaySound(dashAudio);

        while (Vector2.Distance(transform.position, player.position) > 0.5f && dashDistance > 0)
        {
            rb.velocity = direction * dashSpeed;
            yield return null;
            dashDistance -= (direction * dashSpeed * Time.deltaTime).magnitude;
        }

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Chờ hoàn thành animation Attack

        yield return new WaitForSeconds(delayBetweenDashes);
        isDashing = false;
        myTrailRenderer.emitting = false;
    }

    private void PerformAttack(PlayerHealth playerHealth)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                PerformAttack(playerHealth);
            }
        }
    }

    private IEnumerator RetreatAndShoot()
    {
        isShooting = true;
        dashCount = 0;

        Vector2 retreatDirection = (transform.position - player.position).normalized;
        rb.velocity = retreatDirection * moveSpeed;
        yield return new WaitForSeconds(1f);

        rb.velocity = Vector2.zero;

        float angleOffset = 15f; // Góc để tạo ra sóng sin
        float angle = 0f;

        // Phát âm thanh bắn
        PlaySound(shootAudio);

        for (int i = 0; i < projectileCount; i++)
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning("Projectile Prefab is missing or has been destroyed.");
                break;
            }

            angle += angleOffset;
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Sin(angle) * 30f); // Điều chỉnh góc bắn
            Vector2 shootingDirection = rotation * (player.position - transform.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().SetDirection(shootingDirection);

            yield return new WaitForSeconds(shootingCooldown);
        }

        isShooting = false;
    }

    private IEnumerator RandomMovement()
    {
        while (true)
        {
            if (!isDashing && !isChasing && !isShooting)
            {
                animator.SetTrigger("Move");
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.velocity = randomDirection * moveSpeed;
                yield return new WaitForSeconds(Random.Range(1f, 3f));
            }
            yield return null;
        }
    }

    private void OnEnable()
    {
        playerDetected = false; // Reset trạng thái phát hiện player
        dashCount = 0;          // Reset số lần dash
        isDashing = false;
        isShooting = false;
        isChasing = false;

        if (healthBar != null)
        {
            healthBar.ToggleBossHealthBar(false); // Ẩn thanh máu
        }
    }


    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
