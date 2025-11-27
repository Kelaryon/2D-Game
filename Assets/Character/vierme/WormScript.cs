using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WormScript : MonoBehaviour, EnemyInterface
{

    public Boolean facingRight = false;
    [SerializeField] private long maxHealth;
    private long health;

    private Animator animator;
    [SerializeField] private Transform gorundDetection;
    public LayerMask groundLayer;  // Layer mask for the ground
    public LayerMask playerLayer;  // Layer mask for the player
    [SerializeField] private float attackZoneRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float maxHeight;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float invincibilityDurationSeconds;
    private Boolean isInvincible;
    [SerializeField] private Boolean enemyInRange;
    private Rigidbody2D rigidbody2;
    private float jumpRange = 2;
    [SerializeField] private Boolean canAttack;

    public void TakeDamage(long damage, Transform player)
    {
        if (isInvincible) { return; }
        health -= damage;
        if (health <= 0)
        {
            isInvincible = true;
            enabled = false;
            animator.SetTrigger("trDie");
        }
        else
        {
            animator.SetTrigger("trHit");
            StartCoroutine(BecomeTemporarilyInvincible());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        this.animator = GetComponent<Animator>();
        this.rigidbody2 = GetComponent<Rigidbody2D>();
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        HitPlayer();
        if (canAttack)
        {
            Idle();
            if (enemyInRange)
            {
                canAttack = false;
                enemyInRange = false;
                animator.SetTrigger("trAttack");
            }
        }
    }

    private void HitPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackZoneRange, playerLayer);
        if (player != null)
        {
            player.GetComponent<EnemyInterface>().TakeDamage(1, null);
        }
    }
    private void Idle()
    {
        RaycastHit2D raycastHitRight = Physics2D.Raycast(attackPoint.position, Vector2.right, aggroRange, playerLayer);
        RaycastHit2D raycastHitLeft = Physics2D.Raycast(attackPoint.position, Vector2.left, aggroRange, playerLayer);
        enemyInRange = raycastHitLeft || raycastHitRight;
        if (enemyInRange)
        {
            if (raycastHitLeft)
            {
                jumpRange = -3;
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else
            {
                jumpRange = 3;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDurationSeconds);
        isInvincible = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackZoneRange);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + Vector3.left * aggroRange);
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + Vector3.right * aggroRange);
    }

    private void Jump()
    {
        rigidbody2.AddForce(new Vector2(jumpRange, maxHeight), ForceMode2D.Impulse);
    }
    private void FinishAttack()
    {
        rigidbody2.velocity = Vector2.zero;
        StartCoroutine(AttackCooldown());
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2);
        canAttack = true;
    }
}
