using System.Collections;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    #region Variables

    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Dashing,
        Gliding
    }

    [Header("Player Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Damage Feedback")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float knockbackHeight = 3f;
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Stats Config")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.1f;
    [SerializeField] private float dashCooldown = 2f;

    [Header("Wall Movement")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private float wallJumpingPowerX = 8f;
    [SerializeField] private float wallJumpingPowerY = 10f;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallCheckDistance = 0.8f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Stomp")]
    [SerializeField] private float stompBounceHeight = 3f;

    [Header("Push")]
    [SerializeField] private float pushPower = 2f;

    private const float InputDeadzone = 0.01f;

    public PlayerState currentState = PlayerState.Idle;

    private Vector2 moveInput;

    // Knockback
    private float lastKnownHealth;
    private bool isKnockedBack;

    // Movement
    private float horizontalVelocity;
    private float verticalVelocity;

    // Direction
    private float facingDirection = 1f;

    // Jump
    private bool canDoubleJump;

    // Wall movement
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    // Dash
    private bool canDash = true;
    private float dashDirection;

    // Animator
    private int currentAnimationDirection;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (wallLayer.value == 0)
            wallLayer = LayerMask.GetMask("Wall");
    }

    private void Start()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.OnPlayerMove += HandleMovement;
        InputManager.Instance.OnJump += HandleJump;
        InputManager.Instance.OnDash += HandleDash;
        //InputManager.Instance.OnAttack += HandleAttack;

        lastKnownHealth = playerHealth.CurrentHealth;
        playerHealth.OnHealthChanged += HandleHealthChanged;
    }

    private void Update()
    {
        UpdateState();

        UpdateWallMovement();

        if (currentState != PlayerState.Dashing)
            ApplyGravity();

        if (currentState != PlayerState.Attacking)
            ApplyMovement();

        UpdateAnimator();
    }

    #endregion

    #region Player State

    private void UpdateState()
    {
        if (currentState == PlayerState.Dashing)
            return;

        if (currentState == PlayerState.Attacking)
            return;

        if (currentState == PlayerState.Gliding)
        {
            UpdateGlidingState();
            return;
        }

        if (characterController.isGrounded)
        {
            if (Mathf.Abs(horizontalVelocity) > InputDeadzone)
                ChangeState(PlayerState.Moving);
            else
                ChangeState(PlayerState.Idle);

            return;
        }

        ChangeState(PlayerState.Jumping);
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        if (newState == PlayerState.Attacking)
            horizontalVelocity = 0f;

        currentState = newState;
    }

    #endregion

    #region Input

    private void HandleMovement(Vector2 inputDirection)
    {
        moveInput = inputDirection;

        float inputX = Mathf.Clamp(inputDirection.x, -1f, 1f);

        if (Mathf.Abs(inputX) > InputDeadzone)
            facingDirection = Mathf.Sign(inputX);
    }

    private void HandleJump()
    {
        // Wall Jump
        if (isWallSliding)
        {
            PerformWallJump();
            return;
        }

        // Normal Jump
        if (characterController.isGrounded)
        {
            PerformJump();
            return;
        }

        // Double Jump
        if (canDoubleJump)
        {
            PerformJump();
            canDoubleJump = false;
        }
    }

    #endregion

    #region Movement

    private void ApplyMovement()
    {
        if (currentState != PlayerState.Dashing && !isWallJumping && !isKnockedBack)
            horizontalVelocity = moveInput.x * moveSpeed;

        Vector3 movement = new Vector3(horizontalVelocity, verticalVelocity, 0f);

        characterController.Move(movement * Time.deltaTime);
        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        bool isMoving = Mathf.Abs(horizontalVelocity) > InputDeadzone;

        if (isMoving)
            facingDirection = Mathf.Sign(horizontalVelocity);

        FlipAnimation(facingDirection, isMoving);
    }

    #endregion

    #region Gravity

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            canDoubleJump = true;

            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            return;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    #endregion

    #region Jump

    private void PerformJump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        isWallJumping = false;

        //if (animator != null)
        //    animator.SetTrigger("Jump");
    }

    private void PerformWallJump()
    {
        if (wallJumpingCounter <= 0f)
            return;

        isWallJumping = true;
        horizontalVelocity = -wallJumpingDirection * wallJumpingPowerX;
        verticalVelocity = wallJumpingPowerY;
        wallJumpingCounter = 0f;
        facingDirection = Mathf.Sign(horizontalVelocity);

        CancelInvoke(nameof(StopWallJumping));
        Invoke(nameof(StopWallJumping), wallJumpingTime);

        //if (animator != null)
        //    animator.SetTrigger("Jump");
    }

    #endregion

    #region Wall Movement

    private void UpdateWallMovement()
    {
        if (isWallJumping)
        {
            if (verticalVelocity <= 0f)
                StopWallJumping();
        }

        WallSlide();
        WallJump();
    }

    private bool IsWalled(out float wallDirection)
    {
        // Parede à direita
        if (Physics.Raycast(transform.position, Vector3.right, wallCheckDistance, wallLayer))
        {
            wallDirection = 1f;
            return true;
        }

        // Parede à esquerda
        if (Physics.Raycast(transform.position, Vector3.left, wallCheckDistance, wallLayer))
        {
            wallDirection = -1f;
            return true;
        }

        wallDirection = 0f;
        return false;
    }

    private void WallSlide()
    {
        float wallDirection;

        bool touchingWall = IsWalled(out wallDirection);

        if (touchingWall && !characterController.isGrounded && Mathf.Abs(moveInput.x) > InputDeadzone && !isWallJumping)
        {
            isWallSliding = true;
            wallJumpingDirection = wallDirection;
            verticalVelocity = Mathf.Max(verticalVelocity, -wallSlidingSpeed);
            wallJumpingCounter = wallJumpingTime;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            wallJumpingCounter = wallJumpingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;

            if (wallJumpingCounter < 0f)
                wallJumpingCounter = 0f;
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    #endregion

    #region Dash

    private void HandleDash()
    {
        if (!canDash)
            return;

        if (currentState == PlayerState.Attacking)
            return;

        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        ChangeState(PlayerState.Dashing);

        canDash = false;

        // Usa a direção que o personagem está olhando.
        dashDirection = facingDirection;

        horizontalVelocity = dashDirection * dashSpeed;

        // Cancela a velocidade vertical durante o dash.
        verticalVelocity = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            Vector3 dashMovement = new Vector3(horizontalVelocity, 0f, 0f);

            characterController.Move(dashMovement * Time.deltaTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        horizontalVelocity = dashDirection * moveSpeed;

        ChangeState(PlayerState.Idle);

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    #endregion

    #region Glide

    private void UpdateGlidingState()
    {
        // Implementar Glide futuramente.

        // Exemplo:
        //
        // verticalVelocity = -glideFallSpeed;
        //
        // if (characterController.isGrounded)
        // {
        //     ChangeState(PlayerState.Idle);
        // }
    }

    #endregion

    #region Attack

    private void HandleAttack()
    {
        if (currentState == PlayerState.Attacking)
            return;

        if (currentState == PlayerState.Dashing)
            return;

        ChangeState(PlayerState.Attacking);

        //if (animator != null)
        //    animator.SetTrigger("Attack");
    }

    #endregion

    #region Stomp

    public void PerformStompBounce()
    {
        verticalVelocity = Mathf.Sqrt(stompBounceHeight * -2f * gravity);

        canDoubleJump = true;

        //if (animator != null)
        //    animator.SetTrigger("Jump");
    }

    #endregion

    #region Push

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;

        if (hitRigidbody == null || hitRigidbody.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, 0f);

        hitRigidbody.linearVelocity = pushDirection * pushPower;
    }

    #endregion

    #region Damage Feedback

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (currentHealth < lastKnownHealth)
            PerformKnockback();

        lastKnownHealth = currentHealth;
    }

    private void PerformKnockback()
    {
        horizontalVelocity = -facingDirection * knockbackForce;
        verticalVelocity = Mathf.Sqrt(knockbackHeight * -2f * gravity);

        isKnockedBack = true;

        CancelInvoke(nameof(EndKnockback));
        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
    }

    #endregion

    #region Animation

    private void FlipAnimation(float direction, bool isMoving)
    {
        if (animator == null)
            return;

        if (!isMoving)
        {
            // Stopped
            if (currentAnimationDirection != 0)
            {
                //animator.SetInteger("Side", 0);
                currentAnimationDirection = 0;
            }

            return;
        }

        if (direction > 0f)
        {
            // Going Right
            if (currentAnimationDirection != 1)
            {
                //animator.SetInteger("Side", 1);
                currentAnimationDirection = 1;
            }
        }
        else if (direction < 0f)
        {
            // Going Left
            if (currentAnimationDirection != 2)
            {
                //animator.SetInteger("Side", 2);
                currentAnimationDirection = 2;
            }
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
            return;

        float horizontalSpeed = Mathf.Abs(horizontalVelocity);

        //animator.SetFloat("Speed", horizontalSpeed);
        //animator.SetBool("IsGrounded", characterController.isGrounded);
        //animator.SetFloat("VerticalVelocity", verticalVelocity);
        //animator.SetBool("IsWallSliding", isWallSliding);
        //animator.SetBool("IsDashing", currentState == PlayerState.Dashing);
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Right Wall Check
        Gizmos.DrawRay(transform.position, Vector3.right * wallCheckDistance);

        // Left Wall Check
        Gizmos.DrawRay(transform.position, Vector3.left * wallCheckDistance);
    }

    #endregion

    private void OnDestroy()
    {
        CancelInvoke(nameof(StopWallJumping));

        if (InputManager.Instance == null)
            return;

        InputManager.Instance.OnPlayerMove -= HandleMovement;
        InputManager.Instance.OnJump -= HandleJump;
        InputManager.Instance.OnDash -= HandleDash;
        //InputManager.Instance.OnAttack -= HandleAttack;

        playerHealth.OnHealthChanged -= HandleHealthChanged;
    }
}