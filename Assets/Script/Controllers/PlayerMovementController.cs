using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private float _jumpHeight = 5f;
    [SerializeField] private GameObject _groundCheckObject;
    private float _jumpGracePeriod = 0.1f; // The time window in which the player won't check for the ground after jumping.
    private float _jumpGraceTimer = 0f;
    private bool _jumpGracePeriodActive = false;
    private bool _isGrounded = true;
    public bool jumpEnabled = true;

    [Header("Dash Settings")]
    [SerializeField] private float _dashForce = 10f;
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
        // Convert player input into a Vector2 for use to move the player.
        if (_movementInputActionReference != null)
            _movementInputs = _movementInputActionReference.action.ReadValue<Vector2>();

        // Check if the player is on the ground and whether the 'jump' key was pressed. If so, apply a vertical velocity to the player to make them jump.
        if (_jumpInputActionReference.action.ReadValue<float>() > 0 && _isGrounded && jumpEnabled)
        {
            // Check if the player is on the ground to decide whether to allow jumping or not.
            if (Physics2D.OverlapCircle(_groundCheckObject.transform.position, 0.1f, _walkableGroundLayerMask))
            {
                _playerRigidbody2D.linearVelocityY = Mathf.Sqrt((_jumpHeight * 1.2f) * -2f * _gravityValue);
                _jumpGracePeriodActive = true;
            }
        }

        UpdateJumpGracePeriod();

        if (_movementInputs == Vector2.zero && _isGrounded && !_jumpGracePeriodActive)
            _playerRigidbody2D.linearVelocity = Vector2.zero;

        // Check whether the player is not moving and is on the ground. If so, disable gravity to avoid sliding off of slopes. Otherwise, enable gravity to allow the player to fall.
        if (_slopeAngle != 0 && _isGrounded)
            _playerRigidbody2D.gravityScale = 0f;
        else
            _playerRigidbody2D.gravityScale = _originalGravityScale;
    }

    private void FixedUpdate()
    {
        // Move the player horizontally based on the vectors resulting from the inputs, then multiply that by the movement speed value. The 'Y' value of this vector is unused as it is only relevant for climbing.
        Vector2 velocity = new Vector2(_movementInputs.x * _movementSpeed, _playerRigidbody2D.linearVelocity.y);
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
            // If the player is not moving and grounded, set the velocity to zero to avoid a bouncing effect on stairs.

        }
        return velocity;
    }
    #endregion

    #region collision checks with ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // We first shift 1 by the colliding objects layer. Then we check if _walkableGroundLayerMask is equal to that by doing a bitwise OR.
        if ((_walkableGroundLayerMask == (_walkableGroundLayerMask | (1 << collision.gameObject.layer))))
            _isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // We first shift 1 by the colliding objects layer. Then we check if _walkableGroundLayerMask is equal to that by doing a bitwise OR.
        if (_walkableGroundLayerMask == (_walkableGroundLayerMask | (1 << collision.gameObject.layer)))
            _isGrounded = false;
    }

    #endregion

    #region Null reference and jump grace period checks
    private bool IsInputActionReferenceNull()
    {
        // Check if any control inputs are not assigned and return true if none are assigned, otherwise return false.
        return _movementInputActionReference == null &&
            _jumpInputActionReference == null &&
            _dashInputActionReference == null;
    }

    // Adds a graceperiod to perform a successful jump without having interferance with the ground checks.
    private void UpdateJumpGracePeriod()
    {
        if (_jumpGracePeriodActive)
            _jumpGraceTimer += Time.deltaTime;
        if (_jumpGraceTimer >= _jumpGracePeriod)
        {
            _jumpGracePeriodActive = false;
            _jumpGraceTimer = 0f;
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