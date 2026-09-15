using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class OldPlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float _horizontalMovement;
    public float MovementSpeed = 5f;

    [Header("Jump")]
    private bool _isJumping = false;
    private float _jumpTime = 0f;
    private float _maxJumpTime = 0.4f;
    public float JumpForce = 5f;
    public float CoyoteTime = 0.30f;

    [Header("Dash")]
    private bool _canDash = true;
    private bool _isDashing = false;
    private float _dashTimeLimit = 0f;
    private TrailRenderer _trailRenderer;
    public float MaximumDashTime = 1f;
    public float DashSpeedModifier = 20f;
    public float DashCooldown = 1f;
    public LayerMask DashStopLayer;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public Vector2 GroundCheckRadius = new(0.5f, 0.1f);
    public LayerMask GroundLayer;

    [Header("Gravity")]
    public float BaseGravity;
    public float MaxFallSpeed;
    public float FallSpeedMultiplier;

    [Header("Climbing")]
    private float _verticalMovement;
    private float _climbableObjectXPosition; // Store the X position of the climbable object
    private bool _isXPositionLocked = false; // Track if X position is locked during climbing
    private bool _isClimbing = false;
    private bool _climbingEnabled = true;
    public float ClimbSpeed = 5f;
    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    public Vector2 wallCheckRadius = new Vector2(0.2f, 1f);
    public LayerMask climbableLayer;

    [Header("Facing Direction")]
    private float _lastHorizontalInput = 0f;

    [Header("Dialogue")]
    private DialogueManager _dialogueManager;

    [Header("Knockback")]
    private Knockback _knockback;

    [Header("Health")]
    private Health _health;
    
    [Header("Audio")]
    [SerializeField] private float FootstepInterval = 0.20f; // seconds between footsteps
    [SerializeField] private float MinMovementSpeedForFootsteps = 0.1f; // min horizontal speed to consider "moving"
    private float _footstepTimer = 0f;

    [Header("SubCheckpoints")]
    private Vector2 _subCheckPointPosition;

    private float _timeOfLastGroundTouch = 0f;
    private Rigidbody2D _playerRigidbody;
    private CapsuleCollider2D _playerCollider;
    private PlayerAnimationManager _animationManager;
    private InputState _inputState = InputState.Enabled;
    public bool RealGrounded;               //Grounded bool without coyote time
    public Rigidbody2D MovingTileRigidbody;
    public Transform StartPosition;
    public Vector3 LastGroundedPosition;

    private void Awake()
    {
        if (!(TryGetComponent<Rigidbody2D>(out _playerRigidbody) &&
              TryGetComponent<TrailRenderer>(out _trailRenderer) &&
              TryGetComponent<Knockback>(out _knockback) &&
              TryGetComponent<Health>(out _health) &&
              TryGetComponent<Animator>(out var playerAnimator) &&
              this.TryFindFirstObjectByType<DialogueManager>(out _dialogueManager)))
        {
            throw new MissingComponentException("GameObject PlayerController is missing one of the following" +
                                                "components: RigidBody2D, TrialRenderer, Knockback, Health, Animator or DialogueManager.");
        }
        
        _subCheckPointPosition = (Vector2)StartPosition.position;
        
        _animationManager = new PlayerAnimationManager(playerAnimator);
        _animationManager.FacingDirection = Direction.Right;
    }

    private void Update()
    {
        if (_inputState == InputState.Disabled)
        {
            _horizontalMovement = 0;
            _verticalMovement = 0;
        }

        var rotationAngle = 0f;
        RealGrounded = (bool) Physics2D.OverlapBox((Vector2)GroundCheck.position, GroundCheckRadius, rotationAngle, (int)GroundLayer);

        if (_animationManager.IsDashing || _knockback.IsBeingKnockedBack) 
            return;

        if (_isXPositionLocked && _animationManager.IsClimbing)
            _playerRigidbody.linearVelocity = new Vector2(Constants.NoMovement, _playerRigidbody.linearVelocity.y);

        Movement();

        // UpdateAnimationStates();

        Gravity();

        if (IsGrounded())
            _animationManager.IsJumping = false;

        UpdateFootstepAudio();
    }
    
    private void FixedUpdate()
    {
        if (_animationManager.IsDashing || _knockback.IsBeingKnockedBack) 
            return;

        var platformVelocityX = 0f;
        if (MovingTileRigidbody)
            platformVelocityX = MovingTileRigidbody.linearVelocity.x;

        if (_isXPositionLocked && _animationManager.IsClimbing)
            _playerRigidbody.linearVelocity = new Vector2(Constants.NoMovement, _playerRigidbody.linearVelocity.y);
        else
            _playerRigidbody.linearVelocity = new Vector2(_horizontalMovement * MovementSpeed + platformVelocityX, _playerRigidbody.linearVelocity.y);
    }
    
    private void UpdateFootstepAudio()
    {
        const string FootstepSoundEffectName = "Footsteps";
        const float SoundEffectPitch = 0.4f;
        const float StartFootstepTime = 0f;
        
        if (!IsGrounded() || Mathf.Abs(_horizontalMovement) < MinMovementSpeedForFootsteps)
        {
            _footstepTimer = StartFootstepTime;
            return;
        }

        _footstepTimer += Time.deltaTime;
        
        if (_footstepTimer >= FootstepInterval)
        {
            SoundEffectManager.Play(FootstepSoundEffectName, SoundEffectPitch);
            _footstepTimer = StartFootstepTime;
        }
    }

    private void Movement()
    {
        const string HorizontalMovementInputAxis = "Horizontal";
        const string VerticalMovementInputAxis = "Vertical";
        
        if (_inputState == InputState.Disabled) 
            return;
        
        _horizontalMovement = Input.GetAxisRaw(HorizontalMovementInputAxis);
        _verticalMovement = Input.GetAxisRaw(VerticalMovementInputAxis);

        ChangeRotationToMatchMovingDirection();

        Jump();
        Climbing();

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash && _horizontalMovement != 0)
            StartCoroutine(Dash());
    }

    private void Gravity()
    {
        if (_animationManager.IsClimbing) 
            return;
        
        if (_playerRigidbody.linearVelocity.y < 0)
        {
            _playerRigidbody.gravityScale = BaseGravity * FallSpeedMultiplier;
            _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, Mathf.Max(_playerRigidbody.linearVelocity.y, -MaxFallSpeed));
        }
        else
            _playerRigidbody.gravityScale = BaseGravity;
    }

    private void ChangeRotationToMatchMovingDirection()
    {
        const float MovementInputRecognizedThreshold = 0.01f;
        
        if (_inputState == InputState.Enabled && Mathf.Abs(_horizontalMovement) > MovementInputRecognizedThreshold)
        {
            var newFacingDirection = _horizontalMovement > 0 ? 
                Direction.Right : 
                Direction.Left;

            if (newFacingDirection != _animationManager.FacingDirection)
                _animationManager.FacingDirection = newFacingDirection;
        }
        
        _lastHorizontalInput = _horizontalMovement;
    }

    // private void UpdateAnimationStates()
    // {
    //     const float JumpingTheshold = 0.05f;
    //     const float FallingTheshold = -0.05f;
    //     
    //     if (_health.isInvincibleStatus() || _animationManager.IsSitting)
    //         return;
    //
    //     bool startDashAnimation = _animationManager.IsClimbing || Time.time < _dashTimeLimit;
    //
    //     _animationManager.IsWalking = false;
    //     _animationManager.IsJumping = false;
    //     _animationManager.IsFalling = false;
    //     _animationManager.IsClimbing = false;
    //     _animationManager.IsDashing = false;
    //     
    //     if (startDashAnimation)
    //     {
    //         _animationManager.IsDashing = true;
    //         return;
    //     }
    //
    //     bool isAirborne = !IsGrounded() && !_animationManager.IsClimbing;
    //     if (isAirborne)
    //     {
    //         if (_playerRigidbody.linearVelocity.y > JumpingTheshold)
    //             _animationManager.IsJumping = true;
    //         else if (_playerRigidbody.linearVelocity.y < FallingTheshold)
    //             _animationManager.IsFalling = true;
    //         else if (!_animationManager.IsJumping && !_animationManager.IsFalling)
    //             _animationManager.IsJumping = true;
    //     }
    //     else if (_animationManager.IsClimbing)
    //     {
    //         _playerAnimator.SetBool("isClimbing", true);
    //     }
    //     else if (Mathf.Abs(_horizontalMovement) > 0.1f && IsGrounded() && !_isClimbing)
    //     {
    //         _playerAnimator.SetBool("isWalking", true);
    //     }
    // }

    public void Move(InputAction.CallbackContext context)
    {
        //Update last grounded position if the player is really grounded
        if (RealGrounded)
            LastGroundedPosition = transform.position;
        
        Vector2 movementInput = context.ReadValue<Vector2>();
        _horizontalMovement = movementInput.x;
        _verticalMovement = movementInput.y;
    }

    public void Jump()
    {
        cutJumpShort();
        if (!Input.GetButtonDown("Jump")) return;

        // Prevent jumping if dialogue is active OR if player is in dialogue interaction range
        if (_dialogueManager != null && _dialogueManager.IsDialogueActive) return;
        if (DialogueStarter.IsPlayerInAnyDialogueRange()) return;

        if (!IsGrounded() && !_isClimbing) return;


        _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, JumpForce);
        SoundEffectManager.Play("Jump", 0.4f);
        _isJumping = true;
    }
    public void cutJumpShort()
    {
        if (Input.GetButtonUp("Jump") && _playerRigidbody.linearVelocity.y > 0)
        {
            _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y * 0.5f);
        }
    }

    public void Climbing()
    {
        if (!IsClimbable() || !_climbingEnabled)
        {
            StopClimbing();
            return;
        }

        if (_verticalMovement != 0)
        {
            _isClimbing = true;
            _isXPositionLocked = true;
            _playerRigidbody.gravityScale = 0f; // turns off gravity while climbing so the player doesn't fall

            // Lock X position to the climbable object and only allow Y movement
            Vector3 lockedPosition = new Vector3(_climbableObjectXPosition, transform.position.y, transform.position.z);
            transform.position = lockedPosition;
            _playerRigidbody.linearVelocity = new Vector2(0f, _verticalMovement * ClimbSpeed); // Force X velocity to 0
        }

        if (_verticalMovement == 0 && _isClimbing)
        {
            _playerRigidbody.gravityScale = 0f;
            // Keep X position locked and stop Y movement
            Vector3 lockedPosition = new Vector3(_climbableObjectXPosition, transform.position.y, transform.position.z);
            transform.position = lockedPosition;
            _playerRigidbody.linearVelocity = new Vector2(0f, 0f); // Stop all movement
        }

        if (Input.GetButtonDown("Jump") && _isClimbing)
        {
            // Prevent climbing jump if dialogue is active OR if player is in dialogue interaction range
            if (_dialogueManager != null && _dialogueManager.IsDialogueActive) return;
            if (DialogueStarter.IsPlayerInAnyDialogueRange()) return;

            StopClimbing();
            StartCoroutine(ClimbCooldown(0.2f));
            _playerRigidbody.linearVelocity = new Vector2(_playerRigidbody.linearVelocity.x, JumpForce * 1.2f);
        }

        if (IsGrounded())
        {
            StopClimbing();
        }
    }

    private void StopClimbing()
    {
        _isClimbing = false;
        _isXPositionLocked = false; // Unlock X position
        _playerRigidbody.gravityScale = BaseGravity;
    }

    IEnumerator ClimbCooldown(float duration)
    {
        _climbingEnabled = false;
        yield return new WaitForSeconds(duration);
        _climbingEnabled = true;
    }  

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        // Set the minimum time the dash animation should stay active using the configurable duration
        _dashTimeLimit = Time.time + MaximumDashTime;
        _playerAnimator.SetBool("isDashing", true);
        SoundEffectManager.Play("Dash");
        float originalGravity = _playerRigidbody.gravityScale;
        _playerRigidbody.gravityScale = 0f;
        _trailRenderer.emitting = true;
        float dashStartTime = Time.time;
        while (Time.time < dashStartTime + MaximumDashTime)
        {
            _playerRigidbody.linearVelocity = new Vector2(_horizontalMovement * DashSpeedModifier, 0f);

            // Cast a ray in the dash direction
            Vector2 dashDirection = new Vector2(_horizontalMovement, -0.364f).normalized; // -0.364 ≈ tan(20°)
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, 1.5f, DashStopLayer);

            if (hit.collider != null)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                // Stop dash if surface is slanted (not flat or vertical)
                if (angle > 10f && angle < 80f)
                {
                    break;
                }
            }
            yield return null;
        }

        _trailRenderer.emitting = false;
        _playerRigidbody.gravityScale = originalGravity;
        _isDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        _canDash = true;
    }

    private bool IsClimbable()
    {
        Collider2D rightWall = Physics2D.OverlapBox(wallCheckRight.position, wallCheckRadius, 0f, climbableLayer);
        Collider2D leftWall = Physics2D.OverlapBox(wallCheckLeft.position, wallCheckRadius, 0f, climbableLayer);

        if (rightWall != null)
        {
            _climbableObjectXPosition = GetClimbableTileXPosition(rightWall, wallCheckRight.position);
            return true;
        }
        else if (leftWall != null)
        {
            _climbableObjectXPosition = GetClimbableTileXPosition(leftWall, wallCheckLeft.position);
            return true;
        }

        return false;
    }

    private float GetClimbableTileXPosition(Collider2D climbableCollider, Vector3 checkPosition)
    {
        // Check if it's a tilemap
        TilemapCollider2D tilemapCollider = climbableCollider.GetComponent<TilemapCollider2D>();
        if (tilemapCollider != null)
        {
            // Get the tilemap and grid components
            Tilemap tilemap = climbableCollider.GetComponent<Tilemap>();
            Grid grid = tilemap.layoutGrid;

            if (tilemap != null && grid != null)
            {
                // Convert world position to cell position
                Vector3Int cellPosition = grid.WorldToCell(checkPosition);

                // Get the world position of the center of this specific tile
                Vector3 tileWorldPos = grid.CellToWorld(cellPosition);

                // Add half cell size to get the center of the tile
                tileWorldPos.x += grid.cellSize.x * 0.5f;

                return tileWorldPos.x;
            }
        }

        // Fallback for regular colliders (not tilemaps)
        return climbableCollider.transform.position.x;
    }

    // check ground methods

    private bool IsGrounded()
    {
        if (RealGrounded)
        {
            _timeOfLastGroundTouch = Time.time;
        }

        return Time.time - _timeOfLastGroundTouch < CoyoteTime;
    }

    // Visualize ground and wall check areas in the editor when the player is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(GroundCheck.position, GroundCheckRadius);
        Gizmos.DrawCube(wallCheckRight.position, wallCheckRadius);
        Gizmos.DrawCube(wallCheckLeft.position, wallCheckRadius);
    }

    public void ResetAnimations()
    {
        if (_playerAnimator == null) return;

        _playerAnimator.SetBool("isWalking", false);
        _playerAnimator.SetBool("isJumping", false);
        _playerAnimator.SetBool("isFalling", false);
        _playerAnimator.SetBool("isClimbing", false);
        _playerAnimator.SetBool("isDashing", false);
        _playerAnimator.SetBool("isSitting", false);

        _playerAnimator.ResetTrigger("takeDamage");
        _playerAnimator.ResetTrigger("sittingDown");

        _dashTimeLimit = 0f;

    }

    // Method to control player input
    public void SetInputEnabled(bool enabled)
    {
        _inputState = enabled;
    }

    public void SubCheckpoints()
    {
        SubCheckPointRespawn();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        //SubCheckPoint
        if (other.CompareTag("SubCheckPoints"))
        {
            _subCheckPointPosition = other.transform.position;
        }

    }

    public void SubCheckPointRespawn()
    {
        transform.position = _subCheckPointPosition; 
    }
}
