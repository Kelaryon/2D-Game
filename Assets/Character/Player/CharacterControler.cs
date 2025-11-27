using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControler : MonoBehaviour, EnemyInterface
{
    public float moveSpeed = 4f;  // Movement speed of the player
    public float jumpForce = 8f;  // Jump force
    public Transform groundCheck;  // Transform to check if the player is grounded
    public LayerMask groundLayer;  // Layer mask for the ground

    private Rigidbody2D rb;
    private bool isGrounded;
    [SerializeField] private float groundCheckDistance = 0.52f;  // The distance for the Raycast to check
    [SerializeField] private Transform attackZone;
    [SerializeField] private float attackRange;
    public LayerMask enemyLayer;  // Layer mask for the ground
    public LayerMask objectInteract;  // Layer mask for interactableObjects

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color baseColor;
    public Color hitColor;

    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private float playerHealth;
    private Boolean isInvincible;
    private Boolean isAttacking;
    [SerializeField] private List<Image> healthImages;

    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isAttacking)
        {
            return;
        }
        HandleMovement();
        HandleJump();
        Attack();
        DetectInteraction();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void HandleMovement()
    {
        if (isGrounded)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            float speed = moveInput * moveSpeed;
            playerAnimator.SetBool("isMooving", speed != 0);
            rb.velocity = new Vector2(speed, rb.velocity.y);
            if (moveInput != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

            }
        }
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        playerAnimator.SetBool("isGrounded", isGrounded);
        Debug.DrawLine(groundCheck.position, groundCheck.position - (new Vector3(0, 1, 0) * groundCheckDistance), Color.red);
        if (isGrounded)
        {
            playerAnimator.SetFloat("jumpVelocity", rb.velocity.y);
            if (Input.GetButtonDown("Jump"))
            {
                JumpFunction();
            }
            Debug.DrawLine(groundCheck.position, groundCheck.position - (new Vector3(0, 1, 0) * groundCheckDistance), Color.green);
        }
        else
        {
            playerAnimator.SetFloat("jumpVelocity", rb.velocity.y);
            Debug.DrawLine(groundCheck.position, groundCheck.position - (new Vector3(0, 1, 0) * groundCheckDistance), Color.red);
        }

    }

    private void JumpFunction()
    {
        playerAnimator.SetTrigger("isJumping");
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            isAttacking = true;
            rb.velocity = Vector2.zero;
            audioSource.Play();
            Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackZone.position, attackRange, enemyLayer);

            foreach (Collider2D collider in hitEnemy)
            {
                collider.GetComponent<EnemyInterface>().TakeDamage(1, this.transform);
            }
            playerAnimator.SetTrigger("trAttack");
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
        if (attackZone != null)
        {
            Gizmos.DrawWireSphere(attackZone.position, attackRange);
        }
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }

    public void TakeDamage(long damage, Transform player)
    {
        if (isInvincible) { return; }
        playerHealth -= damage;
        Image image = healthImages.Last();
        image.enabled = false;
        healthImages.Remove(image);
        if (playerHealth <= 0)
        {
            enabled = false;
            rb.velocity = Vector2.zero;
            isInvincible = true;
            playerAnimator.SetTrigger("trDie");
        }
        else
        {
            StartCoroutine(BecomeTemporarilyInvincible());
            playerAnimator.SetTrigger("trHit");
        }
    }
    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        for (float i = 0; i < invincibilityDurationSeconds; i += 0.2f)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = baseColor;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void DetectInteraction()
    {
        Collider2D interactebleObject = Physics2D.OverlapCircle(this.transform.position, attackRange, objectInteract);

        if (interactebleObject != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactebleObject.GetComponent<IInteractable>().Interact();
            }
        }
    }
}
