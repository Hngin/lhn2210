using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 2f;
    public float attackDamage = 20f;
    public float knockBackForce = 5f;
    public TrailRenderer myTrailRenderer;
    private Collider2D swordCollider;

    private float lastAttackTime;

    public AudioSource audioSource;
    public AudioClip attackSword;
    public AudioClip impact;

    private bool canAttack = true;

    private void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
        audioSource = GetComponent<AudioSource>();
        LoadSwordData();
    }

    public void SaveSwordData()
    {
        PlayerPrefs.SetFloat("Sword_AttackDamage", attackDamage);
        PlayerPrefs.Save();
    }

    public void LoadSwordData()
    {
        attackDamage = PlayerPrefs.GetFloat("Sword_AttackDamage", 20f);
    }

    public static void ResetSwordData()
    {
        PlayerPrefs.DeleteKey("Sword_AttackDamage");
    }

    private void Update()
    {
        if (canAttack && Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    public void EnableAttack()
    {
        canAttack = true;
    }

    public void DisableAttack()
    {
        canAttack = false;
    }

    private void Attack()
    {
        PlaySound(attackSword);
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
        myTrailRenderer.emitting = true;

        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }

        StartCoroutine(StopTrailAndDisableCollider());
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private IEnumerator StopTrailAndDisableCollider()
    {
        yield return new WaitForSeconds(0.15f);
        myTrailRenderer.emitting = false;

        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canAttack)
        {
            return;
        }

        PlaySound(impact);
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Vector2 knockBackDirection = (collision.transform.position - transform.position).normalized;
            enemyHealth.TakeDamage(attackDamage, knockBackDirection * knockBackForce);
        }
    }
}
