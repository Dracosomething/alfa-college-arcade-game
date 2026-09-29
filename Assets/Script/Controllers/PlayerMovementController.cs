using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
    [SerializeField] private float _dashForceReference = 2.5f;
    private float _dashForce = 1f;
    private bool _canDash = false;
    private bool _isDashing = false;
    private float _dashDuration = 0.3f;
    private float _dashTimer = 0f;
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
        // Convert the player inputs into a Vector2 to use this to move the player.
        if (_movementInputActionReference != null)
            _movementInputs = _movementInputActionReference.action.ReadValue<Vector2>();

        #region Jump and ground in update
        CheckForValidGround();
        // Check if the player is on the ground and whether the 'jump' key was pressed. If so, call Jump().
        if (_jumpInputActionReference.action.ReadValue<float>() > 0 && _isGroundedWithCoyoteTime && jumpEnabled)
            Jump();
        GroundCheckDelay();
        #endregion

        if (_dashInputActionReference.action.triggered && dashEnabled)
            if (!_isGrounded && _canDash && _movementInputs.x != 0)
                _isDashing = true;
        if (_isDashing)
            Dash();

        if (_movementInputs == Vector2.zero && _isGrounded && !_isGoundCheckDelayActive)
            _playerRigidbody2D.linearVelocity = Vector2.zero;

        // Check whether the player is not moving and is on the ground. If so, disable gravity to avoid sliding off of slopes. Otherwise, enable gravity to allow the player to fall.
        if ((_slopeAngle != 0 && _isGrounded) || _isDashing)
            _playerRigidbody2D.gravityScale = 0f;
        else
            _playerRigidbody2D.gravityScale = _originalGravityScale;
    }

    private void FixedUpdate()
    {
        // Move the player horizontally based on the vectors resulting from the inputs, then multiply that by the movement speed value. The 'Y' value of this vector is unused as it is only relevant for climbing.
        Vector2 velocity = new Vector2(_movementInputs.x * _movementSpeed * _dashForce, _playerRigidbody2D.linearVelocity.y);
        _playerRigidbody2D.linearVelocity = AdjustedVelocityToSlope(velocity);
    }
    #endregion

    #region Slope Adjustment

    // Adjusts the velocity of the player to match the slope of the ground they are standing on. This makes it so that the player can walk down the slope without launching off the steps, or launching into the air when walking upwards.
    private Vector2 AdjustedVelocityToSlope(Vector2 velocity)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1f, _walkableGroundLayerMask);
        _slopeAngle = Vector2.Angle(hitInfo.normal, Vector2.up);

        if (hitInfo.collider != null && _slopeAngle != 0 && _isGrounded)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector2.up, hitInfo.normal);
            Vector2 velocityDownwards = slopeRotation * velocity;
            Vector2 velocityUpwards = slopeRotation * new Vector2(velocity.x, -velocity.y);

            // applies the downwards velocity if the player is moving down slopes. or applies the upwards velocity if the player is moving up slopes.
            if (velocityDownwards.y < 0)
                return velocityDownwards;
            else if (velocityUpwards.y > 0)
                return velocityUpwards;
        }
        return velocity;
    }
    #endregion

    #region Jump implementation
    // apply a vertical velocity to the player to make them jump
    private void Jump()
    {
        _playerRigidbody2D.linearVelocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravityValue);
        _isGoundCheckDelayActive = true;
    }
    #endregion

    #region Dash Implementation
    private void Dash()
    {
        Debug.Log("Dashing");
        _dashTimer += Time.deltaTime;
        if (_dashTimer <= _dashDuration && _movementInputs.x != 0)
        {
            _dashForce = _dashForceReference;
            _playerRigidbody2D.linearVelocityY = 0f;
        }
        else
        {
            _isDashing = false;
            _dashTimer = 0f;
            _dashForce = 1f;
        }            
    }
    #endregion

    #region Null reference and gound check delay
    private bool IsInputActionReferenceNull()
    {
        // Check if any control inputs are not assigned and return true if none are assigned, otherwise return false.
        return _movementInputActionReference == null &&
            _jumpInputActionReference == null &&
            _dashInputActionReference == null;
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

    #region Ground check and coyote time
    private void CheckForValidGround()
    {
        // check if the player is grounded.
        if (Physics2D.OverlapCircle(_groundCheckObject.transform.position, 0.1f, _walkableGroundLayerMask))
            _isGrounded = true;
        else
            _isGrounded = false;
        // set a coyote time to allow the player to jump a but after leaving the ground.
        if (!_isGrounded)
        {
            _canDash = true;
            _coyoteTimeTimer += Time.deltaTime;
            if (_coyoteTimeTimer > _coyoteTime)
                _isGroundedWithCoyoteTime = false;
        }
        else
        {
            _canDash = false;
            _coyoteTimeTimer = 0f;
            _isGroundedWithCoyoteTime = true;
        }
    }
    #endregion

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckObject.transform.position, 0.1f);
    }

    #endregion
}