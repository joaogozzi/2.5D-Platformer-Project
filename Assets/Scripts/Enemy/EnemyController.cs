using UnityEngine;

public abstract class EnemyController : MonoBehaviour, IDamageable
{
    #region Variables

    public enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Hurt,
        Dead
    }

    [Header("Enemy Components")]
    [SerializeField] protected CharacterController characterController;
    [SerializeField] protected Animator animator;

    [Header("Stats")]
    [SerializeField] protected float maxHealth = 30f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float gravity = -9.81f;

    [Header("Patrol")]
    [SerializeField] protected float patrolDistance = 3f;

    [Header("Edge Detection")]
    [SerializeField] protected bool useEdgeDetection = true;
    [SerializeField] protected float edgeCheckDistance = 1f;
    [SerializeField] protected float edgeCheckHorizontalOffset = 0.5f;
    [SerializeField] protected LayerMask groundLayer;

    [Header("Hurt")]
    [SerializeField] protected float hurtDuration = 0.4f;

    [Header("Pushable Corpse")]
    [SerializeField] protected GameObject corpsePrefab;
    [SerializeField] protected float corpseSpawnDelay = 0f;

    protected const float InputDeadzone = 0.01f;

    public EnemyState currentState = EnemyState.Idle;

    protected float currentHealth;
    protected Transform player;
    protected Vector3 spawnPosition;

    protected bool isPlayerInDetectionRange;
    protected bool isPlayerInAttackRange;

    protected float horizontalVelocity;
    protected float verticalVelocity;
    protected float facingDirection = 1f;

    protected bool isDead;

    #endregion

    #region Unity Methods

    protected virtual void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;
        spawnPosition = transform.position;
        
        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");
    }

    protected virtual void Update()
    {
        if (currentState == EnemyState.Dead)
            return;

        UpdateState();
        ApplyGravity();
        UpdateAnimator();
    }

    #endregion

    #region Detecção do Player

    public virtual void SetPlayerReference(Transform playerTransform)
    {
        player = playerTransform;
        isPlayerInDetectionRange = true;
    }

    public virtual void ClearPlayerReference()
    {
        isPlayerInDetectionRange = false;
    }

    public virtual void SetPlayerInAttackRange(bool inRange)
    {
        isPlayerInAttackRange = inRange;
    }

    #endregion

    #region State Machine

    protected virtual void UpdateState()
    {
        if (currentState == EnemyState.Hurt)
            return;

        if (isPlayerInAttackRange)
        {
            ChangeState(EnemyState.Attacking);
            Attack();
            return;
        }

        if (isPlayerInDetectionRange)
        {
            ChangeState(EnemyState.Chasing);
            Chase();
            return;
        }

        ChangeState(EnemyState.Patrolling);
        Patrol();
    }

    protected virtual void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
    }

    protected float GetDistanceToPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        return Vector3.Distance(transform.position, player.position);
    }

    #endregion

    #region Behaviors (implementados pelas subclasses)

    protected abstract void Patrol();

    protected abstract void Chase();

    protected abstract void Attack();

    #endregion

    #region Movement

    protected virtual void Move(float direction)
    {
        horizontalVelocity = direction * moveSpeed;

        if (Mathf.Abs(horizontalVelocity) > InputDeadzone)
            facingDirection = Mathf.Sign(horizontalVelocity);

        Vector3 movement = new Vector3(horizontalVelocity, verticalVelocity, 0f);

        characterController.Move(movement * Time.deltaTime);
    }

    protected virtual bool IsEdgeAhead(float direction)
    {
        if (!useEdgeDetection || Mathf.Abs(direction) < InputDeadzone)
            return false;

        Vector3 origin = transform.position + new Vector3(Mathf.Sign(direction) * edgeCheckHorizontalOffset, 0f, 0f);

        bool hasGroundAhead = Physics.Raycast(origin, Vector3.down, edgeCheckDistance, groundLayer);

        return !hasGroundAhead;
    }

    protected virtual void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            return;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    #endregion

    #region Health

    public virtual void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        ChangeState(EnemyState.Hurt);

        CancelInvoke(nameof(RecoverFromHurt));
        Invoke(nameof(RecoverFromHurt), hurtDuration);

        //if (animator != null)
        //    animator.SetTrigger("Hurt");
    }

    protected virtual void RecoverFromHurt()
    {
        if (currentState == EnemyState.Hurt)
            ChangeState(EnemyState.Idle);
    }

    public virtual void Kill()
    {
        if (isDead)
            return;

        currentHealth = 0f;
        Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        ChangeState(EnemyState.Dead);

        CancelInvoke();

        if (characterController != null)
            characterController.enabled = false;

        //if (animator != null)
        //    animator.SetTrigger("Death");

        enabled = false;

        if (corpsePrefab != null)
        {
            if (corpseSpawnDelay > 0f)
                Invoke(nameof(SpawnCorpseAndDestroy), corpseSpawnDelay);
            else
                SpawnCorpseAndDestroy();
        }
    }

    protected virtual void SpawnCorpseAndDestroy()
    {
        Instantiate(corpsePrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    #endregion

    #region Animation

    protected virtual void UpdateAnimator()
    {
        if (animator == null)
            return;

        //animator.SetFloat("Speed", Mathf.Abs(horizontalVelocity));
        //animator.SetBool("IsGrounded", characterController.isGrounded);
        //animator.SetBool("IsChasing", currentState == EnemyState.Chasing);
    }

    #endregion

    #region Debug

    protected virtual void OnDrawGizmosSelected()
    {
        if (!useEdgeDetection)
            return;

        Vector3 origin = transform.position + new Vector3(facingDirection * edgeCheckHorizontalOffset, 0f, 0f);

        Gizmos.color = IsEdgeAhead(facingDirection) ? Color.red : Color.green;
        Gizmos.DrawLine(origin, origin + Vector3.down * edgeCheckDistance);
    }

    #endregion
}