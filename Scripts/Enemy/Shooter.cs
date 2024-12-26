using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public float damage = 10f;
    public float detectionRadius = 15f;
    public float shootingDistance = 5f;
    public float escapeDistance = 1.5f;
    public float wanderRadius = 4.5f;
    private float fireCooldown;
    private Transform player;
    private bool isShooting;
    private bool isWandering;
    private Vector3 lastPosition;
    private float lastFlipXScale;

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
        lastPosition = transform.position;
        lastFlipXScale = transform.localScale.x; // Lưu trạng thái flip ban đầu
    }

    private void Update()
    {
        if (player == null) return; // Dừng nếu không tìm thấy Player

        fireCooldown -= Time.deltaTime;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Đảo hướng sprite chỉ khi đối tượng không đang bắn hoặc quá gần Player
        if (!isShooting && distanceToPlayer > escapeDistance)
        {
            FlipSpriteBasedOnPlayerPosition();
        }

        if (distanceToPlayer < detectionRadius)
        {
            StopWandering();
            if (distanceToPlayer > shootingDistance)
            {
                ChasePlayer();
            }
            else if (distanceToPlayer > escapeDistance)
            {
                MaintainSafeDistance();
                if (fireCooldown <= 0f && !isShooting)
                {
                    StartCoroutine(ShootBurst());
                }
            }
            else if (distanceToPlayer < escapeDistance)
            {
                RunAwayFromPlayer();
            }
        }
        else if (!isWandering)
        {
            StartCoroutine(Wander());
        }
    }

    private void FlipSpriteBasedOnPlayerPosition()
    {
        float directionToPlayer = player.position.x - transform.position.x;
        if (directionToPlayer > 0 && lastFlipXScale < 0)
        {
            transform.localScale = new Vector3(2, 2, 1); // Quay sang phải
            lastFlipXScale = 2;
        }
        else if (directionToPlayer < 0 && lastFlipXScale > 0)
        {
            transform.localScale = new Vector3(-2, 2, 1); // Quay sang trái
            lastFlipXScale = -2;
        }
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * 3f);
    }

    private void MaintainSafeDistance()
    {
        Vector2 randomDirection = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        transform.position = Vector2.MoveTowards(transform.position, randomDirection, Time.deltaTime * 2f);
    }

    private void RunAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, Time.deltaTime * 3f);
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
        isShooting = true;
        for (int i = 0; i < 3; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.2f); // Delay giữa các lần bắn
        }
        fireCooldown = 1f / fireRate;
        yield return new WaitForSeconds(1f); // Delay sau khi bắn
        isShooting = false;
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        GetComponent<Animator>().SetTrigger("Attack");

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rb.velocity = direction * bulletSpeed;
        }
        BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
        }
    }
}
