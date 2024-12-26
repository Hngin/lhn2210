using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float disengageDistance = 15f;

    [Header("Attack Settings")]
    public float meeleRange = 2f;
    public float speed = 3f;
    public float idleTime = 2f;

    [Header("Projectiles")]
    public GameObject handProjectilePrefab;
    public GameObject laserPrefab;

    [Header("Status Flags")]
    private bool isPlayerDetected;
    private bool isInAttackCycle;

    public int damageMeele = 10;
    public int dashDamage = 15;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Entry"); // Start in Entry animation
    }

    private void Update()
    {
        DetectPlayer();

        if (isPlayerDetected && !isInAttackCycle)
        {
            StartCoroutine(AttackCycle());
        }

        if (isPlayerDetected)
        {
            FlipTowardsPlayer();
        }
    }

    // Player detection activation
    void DetectPlayer()
    {
        if (!isPlayerDetected && Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            isPlayerDetected = true;
            animator.SetTrigger("Appear");
           

        }
    }

    // Attack cycle based on player distance
    IEnumerator AttackCycle()
    {
        isInAttackCycle = true;

        // Wait for Appear animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetTrigger("Idle");

        while (true)
        {
            if (Vector3.Distance(transform.position, player.position) <= meeleRange)
            {
                // Meele Attack while player is close
                yield return MeeleAttack();
            }
            else if (Vector3.Distance(transform.position, player.position) > meeleRange && Vector3.Distance(transform.position, player.position) <= disengageDistance)
            {
                // Range Attack cycle (3 times) if player is at a distance
                for (int i = 0; i < 3; i++)
                {
                    yield return RangeAttack();
                    yield return new WaitForSeconds(idleTime);
                }
                yield return LaserCast(); // Laser Cast after Range Attack
            }
            else
            {
                animator.SetTrigger("Idle");
            }
        }
    }

    IEnumerator MeeleAttack()
    {
        animator.SetTrigger("Meele");

        while (Vector3.Distance(transform.position, player.position) <= meeleRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            yield return null; // Loop until player moves out of range
        }

        // Execute Meele attack animation and damage player each time it loops
        player.GetComponent<PlayerHealth>().TakeDamage(damageMeele);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    IEnumerator RangeAttack()
    {
        animator.SetTrigger("Range");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        GameObject handProjectile = Instantiate(handProjectilePrefab, transform.position, Quaternion.identity);
        handProjectile.GetComponent<HandProjectile>().Initialize(player.position);

        // Dash through player
        Vector3 dashTarget = player.position;
        while (Vector3.Distance(transform.position, dashTarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, speed * Time.deltaTime);
            yield return null;
        }

        // Damage player if the boss dashes through them
        if (Vector3.Distance(transform.position, player.position) <= 1f)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(dashDamage);
        }
    }

    IEnumerator LaserCast()
    {
        animator.SetTrigger("Laser");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.GetComponent<Laser>().Initialize(player.position);
    }

    // Adjust the boss orientation toward the player
    void FlipTowardsPlayer()
    {
        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
