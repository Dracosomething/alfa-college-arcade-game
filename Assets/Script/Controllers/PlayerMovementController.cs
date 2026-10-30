using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    #region Fields
    private const int DashStartTimerValue = 0;
    private const float DashDurationDividerInHalf = 0.5f;
    private const float FlatSlopeAngle = 0f; // The maximum slope angle the player can walk on.
    private const int ZeroUpVelocity = 0;
    private const float GroundCheckRadius = 0.15f;
    private const float GroundCheckDelayLength = 0.1f; // In seconds
    private const float DashDuration = 0.3f; // In seconds

    [Header("Input Action References")]
    [RequiredSerializeField] [SerializeField] private InputActionReference _movementInputActionReference;
    [RequiredSerializeField] [SerializeField] private InputActionReference _jumpInputActionReference;
    [RequiredSerializeField] [SerializeField] private InputActionReference _dashInputActionReference;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 10f;
    [RequiredSerializeField] [SerializeField] private GameObject _groundCheckObject;

    [Header("Dash Settings")]
    [SerializeField] private float _dashForce = 5f;
    [SerializeField] private float _dashCooldown = 1f; // In seconds
    [RequiredSerializeField] [SerializeField] private LayerMask _walkableGroundLayerMask;

    private float _groundCheckDelayTimer = 0f; // In seconds
    private bool _isGroundCheckDelayActive = false;
    private bool _isGrounded = true;

    private const float _coyoteTime = 0.1f; // In seconds
    private float _coyoteTimeTimer = 0f; // In seconds
    private bool _isGroundedWithCoyoteTime = false;

    private bool _canDash = false;
    private bool _isDashButtonPressed = false;
    private float _dashTimer = 0f; // In seconds
    private float _timeSinceLastDash = 0f; // In seconds
    private float _originalGravityScale;

    [Component] private Rigidbody2D _playerRigidbody2D;

    private Vector2 _movementInputs; // Store the movement input values to be used in FixedUpdate
    private float _slopeAngle;
    [Component] private PlayerAnimator _playerAnimator;
    #endregion

    #region Initialization
    private void Start() =>
        // Store the original gravity scale of the player to avoid sliding off of slopes.
        _originalGravityScale = _playerRigidbody2D.gravityScale;
    #endregion

    #region enable, disable
    public void EnablePlayerMovementController()
    {
        _movementInputActionReference.action.Enable();
        _jumpInputActionReference.action.Enable();
        _dashInputActionReference.action.Enable();
    }

    public void DisablePlayerMovementController()
    {
        _movementInputActionReference.action.Disable();
        _jumpInputActionReference.action.Disable();
        _dashInputActionReference.action.Disable();
    }
    #endregion
    
    #region Enable/Disable Input Actions
    private void OnEnable()
    {
        // Enable the inputs of the player when the player is enabled.
        _movementInputActionReference.action.Enable();
        _jumpInputActionReference.action.Enable();
        _dashInputActionReference.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the inputs of the player when the player is disabled in order to avoid memory leaks and unexpected behavior.
        _movementInputActionReference.action.Disable();
        _jumpInputActionReference.action.Disable();
        _dashInputActionReference.action.Disable();
    }
    #endregion

    #region Update Methods
    private void Update()
    {
        _movementInputs = _movementInputActionReference.action.ReadValue<Vector2>();

        JumpUpdate();

        DashUpdate();

        if (_movementInputs.Equals(Vector2.zero) && _isGrounded && !_isGroundCheckDelayActive)
            _playerRigidbody2D.linearVelocity = Vector2.zero;

        if ((!_slopeAngle.Equals(FlatSlopeAngle) && _isGrounded) || _isDashButtonPressed)
            _playerRigidbody2D.gravityScale = Constants.ZeroGravity;
        else
            _playerRigidbody2D.gravityScale = _originalGravityScale;
    }

    private void FixedUpdate()
    {
        Vector2 velocity;
        if (_isDashButtonPressed)
            // using mathf.round to avoid moving on the slightest input from, for example, a controller stick.
            velocity = new Vector2(Mathf.Round(_movementInputs.x) * _movementSpeed * _dashForce, ZeroUpVelocity);
        else
            // using mathf.round to avoid moving on the slightest input from, for example, a controller stick.
            velocity = AdjustedVelocityToSlope(new Vector2(Mathf.Round(_movementInputs.x) * _movementSpeed, _playerRigidbody2D.linearVelocity.y));

        _playerRigidbody2D.linearVelocity = velocity;
        _playerAnimator.GetMovementAndGroundValues(_movementInputs, velocity, _isGrounded);
    }
    #endregion

    #region Slope Adjustment
    private Vector2 AdjustedVelocityToSlope(Vector2 velocity)
    {
        float rayLength = 2f; // The length of the ray used for slope detection.
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, rayLength, _walkableGroundLayerMask);
        _slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);

        if (!hitInfo.collider.IsUnityNull() && !_slopeAngle.Equals(FlatSlopeAngle) && _isGrounded)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector2.up, hitInfo.normal);
            velocity = GetVelocityUpOrDown(velocity, slopeRotation);
        }

        return velocity;
    }

    private Vector2 GetVelocityUpOrDown(Vector2 velocity, Quaternion slopeRotation)
    {
        Vector2 velocityDownwards = slopeRotation * velocity;
        Vector2 velocityUpwards = slopeRotation * new Vector2(velocity.x, -velocity.y);

        if (velocityDownwards.y < ZeroUpVelocity)
            return velocityDownwards;

        if (velocityUpwards.y > ZeroUpVelocity)
            return velocityUpwards;

        return velocity;
    }
    #endregion

    #region Jump implementation
    private void JumpUpdate()
    {
        CheckForValidGround();

        if (_jumpInputActionReference.action.ReadValue<float>() > Constants.NoInput && _isGroundedWithCoyoteTime)
            Jump();

        GroundCheckDelay();
    }


    private void Jump()
    {
        const float JumpVelocityGravityFactor = -2f;
        // JumpVelocityGravityFactor determines the relationship between jump height and gravity. It maked the calculation of the jump velocity based on the desired jump height and gravity value.
        _playerRigidbody2D.linearVelocityY = Mathf.Sqrt(_jumpHeight * JumpVelocityGravityFactor * Constants.DefaultGravity);
        _isGroundCheckDelayActive = true;
    }
    #endregion

    #region Dash Implementation
    private void DashUpdate()
    {
        const float MovementInputThreshold = 0.5f;
        const int LastDashResetValue = 0;

        _timeSinceLastDash += Time.deltaTime;
        // checking if the input was pressed and if the player can dash and the cooldown is over, then check if then player can actually dash by checking if its grounded and movements aren't null.
        if (_dashInputActionReference.action.WasPressedThisFrame() && _timeSinceLastDash >= _dashCooldown &&
            !_isGrounded && _canDash && (_movementInputs.x > MovementInputThreshold || _movementInputs.x < -MovementInputThreshold))
        {
            if (HasCollisionWithObject())
                return;

            _isDashButtonPressed = true;
            _timeSinceLastDash = LastDashResetValue;
        }

        if (_dashInputActionReference.action.WasReleasedThisFrame())
        {
            if (_dashTimer > (DashDuration * DashDurationDividerInHalf))
            {
                _isDashButtonPressed = false;
                _canDash = false;
                _dashTimer = DashStartTimerValue;
            }
        }

        if (_isDashButtonPressed)
            Dash();
    }

    private void Dash()
    {
        _dashTimer += Time.deltaTime;

        if (_dashTimer > DashDuration)
        {
            _isDashButtonPressed = false;
            _canDash = false;
            _dashTimer = DashStartTimerValue;
        }
    }
    #endregion

    #region Ground check delay and coyote time
    private void CheckForValidGround()
    {
        const int CoyoteTimeTimerStartValue = 0;
        _isGrounded = false;

        if (Physics2D.OverlapCircle(_groundCheckObject.transform.position, GroundCheckRadius, _walkableGroundLayerMask))
            _isGrounded = true;

        // set a coyote time to allow the player to jump a bit after leaving the ground.
        if (!_isGrounded)
        {
            _coyoteTimeTimer += Time.deltaTime;

            if (_coyoteTimeTimer > _coyoteTime)
                _isGroundedWithCoyoteTime = false;

            return;
        }

        _canDash = true;
        _coyoteTimeTimer = CoyoteTimeTimerStartValue;
        _isGroundedWithCoyoteTime = true;
    }

    // Adds a delay to check for the ground to perform a successful jump.
    private void GroundCheckDelay()
    {
        const int GroundCheckDelayTimerStartValue = 0;

        if (_isGroundCheckDelayActive)
            _groundCheckDelayTimer += Time.deltaTime;

        if (_groundCheckDelayTimer >= GroundCheckDelayLength)
        {
            _isGroundCheckDelayActive = false;
            _groundCheckDelayTimer = GroundCheckDelayTimerStartValue;
        }
    }
    #endregion

    #region collision checks with walls while dashing
    // Avoiding dashing into walls by stopping the dash.
    private bool HasCollisionWithObject()
    {
        const int ContactPointsArraySize = 1;
        const int FirstContactPoint = 0;

        ContactPoint2D[] contactPoints = new ContactPoint2D[ContactPointsArraySize];
        _playerRigidbody2D.GetContacts(contactPoints);

        return !contactPoints[FirstContactPoint].collider.IsUnityNull();
    }
    #endregion
}