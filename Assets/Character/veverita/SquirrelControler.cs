using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SquirrelControler : MonoBehaviour,EnemyInterface
{

    public Boolean facingRight = true;
    [SerializeField] private long maxHealth;
    private long health;
    [SerializeField] private long speed;

    private Animator animator;
    [SerializeField] private Transform gorundDetection;
    public LayerMask groundLayer;  // Layer mask for the ground

    [SerializeField] private float invincibilityDurationSeconds;
    private Boolean isInvincible;
    [SerializeField] private Boolean isAtacked;
    private Transform attacker;

    void Start()
    {
        health = maxHealth;
        this.animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isAtacked)
        {
            RunAway();
        }
    }
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
            attacker = player;
            animator.SetTrigger("trHit");
            StartCoroutine(BecomeTemporarilyInvincible());
        }
    }

    private void SetFacingDirection(Transform player)
    {
        Vector2 toTarget = player.position - this.transform.position;
        if (toTarget.x < 0){
            facingRight = true;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            facingRight = false;
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
    }

    public void DeathAnimationEnd()
    {
        animator.enabled = false;
    }
    private void CheckEnviromentCollision()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Boolean isGrounded = Physics2D.Raycast(gorundDetection.position, Vector2.down, 0.52f, groundLayer);
        Boolean hasHitAWall = Physics2D.Raycast(gorundDetection.position, dir, 0.52f, groundLayer);
        Debug.DrawLine(gorundDetection.position, gorundDetection.position + (Vector3)Vector2.down * 0.52f, Color.red);
        Debug.DrawLine(gorundDetection.position, gorundDetection.position + (Vector3)dir * 0.52f, Color.red);
        if (!isGrounded || hasHitAWall)
        {
            if (facingRight)
            {
                facingRight = !facingRight;
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else
            {
                facingRight = !facingRight;
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
    private void RunAway()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Vector3 move = speed * Time.deltaTime * dir;
        transform.Translate(move, Space.World);
        animator.SetBool("boolRun", true);
        CheckEnviromentCollision();
    }
    public void SetIsAttacked()
    {
        SetFacingDirection(attacker);
        isAtacked = true;
    }
}
