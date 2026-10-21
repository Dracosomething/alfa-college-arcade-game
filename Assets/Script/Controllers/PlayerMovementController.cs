using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    #region Fields
    private const float NoInputs = 0f;
    private const int DashStartTimerValue = 0;
    private const float DashDurationDividerInHalf = 0.5f;
    private const float FlatSlopeAngle = 0f; // The maximum slope angle the player can walk on.
    private const int ZeroUpVelocity = 0;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference _movementInputActionReference;
    [SerializeField] private InputActionReference _jumpInputActionReference;
    [SerializeField] private InputActionReference _dashInputActionReference;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 10f;
    [SerializeField] private GameObject _groundCheckObject;

    [Header("Dash Settings")]
    [SerializeField] private float _dashForce = 5f;
    [SerializeField] private float _dashCooldown = 1f; // In seconds

    private float _groundCheckRadius = 0.15f;
    private float _groundCheckDelayLength = 0.1f; // In seconds
    private float _groundCheckDelayTimer = 0f; // In seconds
    private bool _isGroundCheckDelayActive = false;
    private bool _isGrounded = true;

    private float _coyoteTime = 0.1f; // In seconds
    private float _coyoteTimeTimer = 0f; // In seconds
    private bool _isGroundedWithCoyoteTime = false;

    private bool _canDash = false;
    private bool _isDashButtonPressed = false;
    private float _dashDuration = 0.3f; // In seconds
    private float _dashTimer = 0f; // In seconds
    private float _timeSinceLastDash = 0f; // In seconds

    private float _gravityValue = -9.81f;
    private float _originalGravityScale;

    [SerializeField] private LayerMask _walkableGroundLayerMask;
    private Rigidbody2D _playerRigidbody2D;

    private Vector2 _movementInputs; // Store the movement input values to be used in FixedUpdate
    private float _slopeAngle;
    #endregion

    #region Initialization
    private void Awake()
    {
        if (IsInputActionReferenceNull())
            throw new MissingReferenceException("Input Action References are not assigned in the inspector. Please assign them in the inspector.");

        if (!TryGetComponent<Rigidbody2D>(out _playerRigidbody2D))
            throw new MissingComponentException("Rigidbody2D component is missing from the GameObject. Please add a Rigidbody2D component.");
        // Store the original gravity scale of the player to avoid sliding off of slopes.
        _originalGravityScale = _playerRigidbody2D.gravityScale;
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

        if (_movementInputs == Vector2.zero && _isGrounded && !_isGroundCheckDelayActive)
            _playerRigidbody2D.linearVelocity = Vector2.zero;

        if ((_slopeAngle != FlatSlopeAngle && _isGrounded) || _isDashButtonPressed)
            _playerRigidbody2D.gravityScale = Constants.ZeroGravity;
        else
            _playerRigidbody2D.gravityScale = _originalGravityScale;
    }

    private void FixedUpdate()
    {
        if (_isDashButtonPressed)
            // using mathf.round to avoid moving on the slightest input from, for example, a controller stick.
            _playerRigidbody2D.linearVelocity = new Vector2(Mathf.Round(_movementInputs.x) * _movementSpeed * _dashForce, ZeroUpVelocity);
        else
            // using mathf.round to avoid moving on the slightest input from, for example, a controller stick.
            _playerRigidbody2D.linearVelocity = AdjustedVelocityToSlope(new Vector2(Mathf.Round(_movementInputs.x) * _movementSpeed, _playerRigidbody2D.linearVelocity.y));
    }
    #endregion

    #region Slope Adjustment
    private Vector2 AdjustedVelocityToSlope(Vector2 velocity)
    {
        float rayLength = 2f; // The length of the ray used for slope detection.
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, rayLength, _walkableGroundLayerMask);
        _slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);

        if (!hitInfo.collider.IsUnityNull() && _slopeAngle != FlatSlopeAngle && _isGrounded)
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
        else if (velocityUpwards.y > ZeroUpVelocity)
            return velocityUpwards;
        return velocity;
    }
    #endregion

    #region Jump implementation
    private void JumpUpdate()
    {
        CheckForValidGround();

        if (_jumpInputActionReference.action.ReadValue<float>() > NoInputs && _isGroundedWithCoyoteTime)
            Jump();

        GroundCheckDelay();
    }


    private void Jump()
    {
        const float JumpVelocityGravityFactor = -2f;
        // JumpVelocityGravityFactor determines the relationship between jump height and gravity. It maked the calculation of the jump velocity based on the desired jump height and gravity value.
        _playerRigidbody2D.linearVelocityY = Mathf.Sqrt(_jumpHeight * JumpVelocityGravityFactor * _gravityValue);
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
            !_isGrounded && _canDash && (_movementInputs.x < MovementInputThreshold || _movementInputs.x > -MovementInputThreshold))
        {
            _isDashButtonPressed = true;
            _timeSinceLastDash = LastDashResetValue;
        }

        if (_dashInputActionReference.action.WasReleasedThisFrame())
        {
            if (_dashTimer > (_dashDuration * DashDurationDividerInHalf))
            {
                _isDashButtonPressed = false;
                _canDash = false;
                _dashTimer = DashStartTimerValue;
            }
            else
                _dashTimer += _dashDuration * DashDurationDividerInHalf;
        }

        if (_isDashButtonPressed)
            Dash();
    }

    private void Dash()
    {
        _dashTimer += Time.deltaTime;
        if (_dashTimer > _dashDuration)
        {
            _isDashButtonPressed = false;
            _canDash = false;
            _dashTimer = DashStartTimerValue;
        }
    }
    #endregion

    #region Null references
    private bool IsInputActionReferenceNull() =>
        _movementInputActionReference.IsUnityNull() &&
        _jumpInputActionReference.IsUnityNull() &&
        _dashInputActionReference.IsUnityNull();
    #endregion

    #region Ground check delay and coyote time
    private void CheckForValidGround()
    {
        const int CoyoteTimeTimerStartValue = 0;
        _isGrounded = false;

        if (Physics2D.OverlapCircle(_groundCheckObject.transform.position, _groundCheckRadius, _walkableGroundLayerMask))
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

        if (_groundCheckDelayTimer >= _groundCheckDelayLength)
        {
            _isGroundCheckDelayActive = false;
            _groundCheckDelayTimer = GroundCheckDelayTimerStartValue;
        }
    }
    #endregion

    #region collision checks with walls while dashing
    // Avoiding dashing into walls by stopping the dash.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // We first shift 1 by the colliding objects layer. Then we check if _walkableGroundLayerMask is equal to that by doing a bitwise OR.
        if (_walkableGroundLayerMask == (_walkableGroundLayerMask | (1 << collision.gameObject.layer)))
        {
            _isDashButtonPressed = false;
            _canDash = false;
            _dashTimer = DashStartTimerValue;
        }
    }
    #endregion

    #region enable, disable
    public void EnablePlayerMovementController(bool enabled)
    {
        if (enabled)
        { 
            _movementInputActionReference.action.Enable();
            _jumpInputActionReference.action.Enable();
            _dashInputActionReference.action.Enable();
        }
        else
        {
            _movementInputActionReference.action.Disable();
            _jumpInputActionReference.action.Disable();
            _dashInputActionReference.action.Disable();
        }
    }
    #endregion
}