using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    #region Fields
    [Header("Input Action References")]
    [SerializeField] private InputActionReference _movementInputActionReference;
    [SerializeField] private InputActionReference _jumpInputActionReference;
    [SerializeField] private InputActionReference _dashInputActionReference;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 5f;
    private Vector2 _movementInputs; // Store the movement input values to be used in FixedUpdate
    private float _slopeAngle;
    public bool moveEnabled = true;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 10f;
    [SerializeField] private GameObject _groundCheckObject;
    private float _groundCheckDelayLenght = 0.1f; // The time window in which the player won't check for the ground after jumping.
    private float _groundCheckDelayTimer = 0f;
    private bool _isGoundCheckDelayActive = false;
    private bool _isGrounded = true;
    public bool jumpEnabled = true;

    [Header("Coyote Time settings")]
    private float _coyoteTime = 0.1f;
    private float _coyoteTimeTimer = 0f;
    private bool _isGroundedWithCoyoteTime = false;

    [Header("Dash Settings")]
    [SerializeField] private float _dashForce = 5f;
    [SerializeField] private float _dashCooldown = 1f;
    private bool _canDash = false;
    private bool _isDashing = false;
    private float _dashDuration = 0.3f;
    private float _dashTimer = 0f;
    private float _timeSinceLastDash = 0f;
    public bool dashEnabled = true;

    [Header("Gravity Settings")]
    private float _gravityValue = -9.81f;
    private float _originalGravityScale;

    [Header("Miscellaneous Settings")]
    [SerializeField] private LayerMask _walkableGroundLayerMask;
    private Rigidbody2D _playerRigidbody2D;
    #endregion
    #region Initialization
    private void Awake()
    {
        if (IsInputActionReferenceNull())
            throw new MissingReferenceException("Input Action References are not assigned in the inspector. Please assign them in the inspector.");

        if (!TryGetComponent<Rigidbody2D>(out _playerRigidbody2D))
            throw new MissingComponentException("Rigidbody2D component is missing from the GameObject. Please add a Rigidbody2D component.");
        else
            // Store the original gravity scale of the player to advoid sliding off of slopes.
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
        // avoiding endless errors in case the input action references are not assigned in the inspector.
        if (!_movementInputActionReference.IsUnityNull())
            _movementInputs = _movementInputActionReference.action.ReadValue<Vector2>();

        #region Jump and ground in update
        CheckForValidGround();
        if (_jumpInputActionReference.action.ReadValue<float>() > 0 && _isGroundedWithCoyoteTime && jumpEnabled)
            Jump();
        GroundCheckDelay();
        #endregion
        #region Dash in update
        _timeSinceLastDash += Time.deltaTime;
        if (_dashInputActionReference.action.WasPressedThisFrame() && dashEnabled && _timeSinceLastDash >= _dashCooldown)
            if (!_isGrounded && _canDash && (_movementInputs.x < 0.5f || _movementInputs.x > -0.5f))
            {
                _isDashing = true;
                _timeSinceLastDash = 0f;
            }

        if (_dashInputActionReference.action.WasReleasedThisFrame() && dashEnabled)
        {
            if (_dashTimer > (_dashDuration * 0.5f))
            {
                _isDashing = false;
                _canDash = false;
                _dashTimer = 0f;
            }
            else
            {
                _dashTimer += _dashDuration * 0.5f;
            }
        }
        if (_isDashing)
            Dash();
        #endregion
        if (_movementInputs == Vector2.zero && _isGrounded && !_isGoundCheckDelayActive)
            _playerRigidbody2D.linearVelocity = Vector2.zero;

        if ((_slopeAngle != 0 && _isGrounded) || _isDashing)
            _playerRigidbody2D.gravityScale = 0f;
        else
            _playerRigidbody2D.gravityScale = _originalGravityScale;
    }

    private void FixedUpdate()
    {
        Vector2 velocity;
        if (_isDashing)
        {
            velocity = new Vector2(Mathf.Round(_movementInputs.x) * _movementSpeed * _dashForce, 0f);
            _playerRigidbody2D.linearVelocity = velocity;
        }
        else
        {
            velocity = new Vector2(Mathf.Round(_movementInputs.x) * _movementSpeed, _playerRigidbody2D.linearVelocity.y);
            _playerRigidbody2D.linearVelocity = AdjustedVelocityToSlope(velocity);
        }
    }
    #endregion
    #region Slope Adjustment

    private Vector2 AdjustedVelocityToSlope(Vector2 velocity)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 2f, _walkableGroundLayerMask);
        _slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);

        if (!hitInfo.collider.IsUnityNull() && _slopeAngle != 0 && _isGrounded)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector2.up, hitInfo.normal);
            Vector2 velocityDownwards = slopeRotation * velocity;
            Vector2 velocityUpwards = slopeRotation * new Vector2(velocity.x, -velocity.y);

            if (velocityDownwards.y < 0)
                return velocityDownwards;
            else if (velocityUpwards.y > 0)
                return velocityUpwards;
        }
        return velocity;
    }
    #endregion
    #region Jump implementation
    private void Jump()
    {
        _playerRigidbody2D.linearVelocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravityValue);
        _isGoundCheckDelayActive = true;
    }
    #endregion
    #region Dash Implementation
    private void Dash()
    {
        _dashTimer += Time.deltaTime;
        if (_dashTimer > _dashDuration)
        {
            _isDashing = false;
            _canDash = false;
            _dashTimer = 0f;
        }
    }
    #endregion
    #region Null references
    private bool IsInputActionReferenceNull()
    {
        return _movementInputActionReference.IsUnityNull() &&
            _jumpInputActionReference.IsUnityNull() &&
            _dashInputActionReference.IsUnityNull();
    }
    #endregion
    #region Ground check delay and coyote time
    private void CheckForValidGround()
    {
        if (Physics2D.OverlapCircle(_groundCheckObject.transform.position, 0.15f, _walkableGroundLayerMask))
            _isGrounded = true;
        else
            _isGrounded = false;
        // set a coyote time to allow the player to jump a bit after leaving the ground.
        if (!_isGrounded)
        {
            _coyoteTimeTimer += Time.deltaTime;
            if (_coyoteTimeTimer > _coyoteTime)
                _isGroundedWithCoyoteTime = false;
        }
        else
        {
            _canDash = true;
            _coyoteTimeTimer = 0f;
            _isGroundedWithCoyoteTime = true;
        }
    }

    // Adds a delay to check for the ground to perform a successful jump.
    private void GroundCheckDelay()
    {
        if (_isGoundCheckDelayActive)
            _groundCheckDelayTimer += Time.deltaTime;
        if (_groundCheckDelayTimer >= _groundCheckDelayLenght)
        {
            _isGoundCheckDelayActive = false;
            _groundCheckDelayTimer = 0f;
        }
    }

    #endregion
    #region collision checks with walls while dashing
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // We first shift 1 by the colliding objects layer. Then we check if _walkableGroundLayerMask is equal to that by doing a bitwise OR.
        if (_walkableGroundLayerMask == (_walkableGroundLayerMask | (1 << collision.gameObject.layer)))
        {
            _isDashing = false;
            _canDash = false;
            _dashTimer = 0f;
        }
    }
    #endregion
}