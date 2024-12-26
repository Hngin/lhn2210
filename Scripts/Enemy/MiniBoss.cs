using UnityEngine;
using System.Collections;

public class MiniBoss : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float damage = 10f;
    public float detectionRadius = 15f;
    public float wanderRadius = 4.5f;
    private float fireCooldown = 0f;
    private Transform player;
    private bool isPlayerDetected = false;
    private bool isWandering;
    private bool isMoving;
    private Vector3 lastPosition;
    private float lastFlipXScale;

    public AudioSource shootingAudioSource;
    public AudioClip shootingClip;
    public AudioClip randomSoundClip;

    private void Start()
    {
        // Tự động tìm đối tượng Player dựa trên tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure the player has the tag 'Player'.");
        }

        StartCoroutine(Wander());
        StartCoroutine(PlayRandomSound());
        lastPosition = transform.position;
        lastFlipXScale = transform.localScale.x; // Lưu trạng thái flip ban đầu
    }

    private void Update()
    {
        if (player == null) return; // Dừng nếu không tìm thấy Player

        fireCooldown -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Đảo hướng sprite khi MiniBoss di chuyển
        FlipSpriteBasedOnMovement();

        if (distanceToPlayer < detectionRadius)
        {
            isPlayerDetected = true; // Đánh dấu rằng đã phát hiện Player
        }

        if (isPlayerDetected)
        {
            StopWandering();
            MoveTowardsPlayer(distanceToPlayer);

            if (fireCooldown <= 0f)
            {
                StartCoroutine(ShootBurst());
                fireCooldown = 2f; // Reset lại thời gian hồi chiêu
            }
        }
        else if (!isWandering)
        {
            StartCoroutine(Wander());
        }
    }

    private void FlipSpriteBasedOnMovement()
    {
        float direction = transform.position.x - lastPosition.x;
        if (direction > 0 && lastFlipXScale < 0)
        {
            transform.localScale = new Vector3(5, 5, 1); // Quay sang phải
            lastFlipXScale = 4;
        }
        else if (direction < 0 && lastFlipXScale > 0)
        {
            transform.localScale = new Vector3(-5, 5, 1); // Quay sang trái
            lastFlipXScale = -4;
        }
        lastPosition = transform.position;
    }

    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * 3f);

        // Gây sát thương khi chạm vào Player
        if (distanceToPlayer <= 1f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Đánh dấu MiniBoss đang di chuyển
        isMoving = true;
    }

    private IEnumerator Wander()
    {
        isWandering = true;
        while (true)
        {
            Vector2 newPosition = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
            float wanderTime = Random.Range(1f, 4f);
            float elapsedTime = 0f;

            while (elapsedTime < wanderTime)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPosition, Time.deltaTime * 2f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void StopWandering()
    {
        isWandering = false;
        StopCoroutine(Wander());
    }

    private IEnumerator ShootBurst()
    {
        isMoving = false; // Dừng âm thanh di chuyển khi bắn

        if (shootingAudioSource != null && shootingClip != null)
        {
            shootingAudioSource.PlayOneShot(shootingClip);
        }
        ShootBulletsAtDirections(new int[] { 2, 4, 6, 8, 10, 12 });
        yield return new WaitForSeconds(0.5f); // Delay giữa các lần bắn
        if (shootingAudioSource != null && shootingClip != null)
        {
            shootingAudioSource.PlayOneShot(shootingClip);
        }
        ShootBulletsAtDirections(new int[] { 1, 3, 5, 7, 9, 11 });
    }

    private void ShootBulletsAtDirections(int[] directions)
    {
        foreach (int direction in directions)
        {
            float angle = direction * 30f; // Mỗi hướng cách nhau 30 độ
            Vector2 bulletDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = bulletDirection * bulletSpeed;
            }
            BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
            }
        }
    }

    private IEnumerator PlayRandomSound()
    {
        while (true)
        {
            float randomDelay = Random.Range(3f, 10f); // Thời gian ngẫu nhiên giữa các lần phát âm thanh
            yield return new WaitForSeconds(randomDelay);

            if (shootingAudioSource != null && randomSoundClip != null)
            {
                shootingAudioSource.PlayOneShot(randomSoundClip);
            }
        }
    }
}
