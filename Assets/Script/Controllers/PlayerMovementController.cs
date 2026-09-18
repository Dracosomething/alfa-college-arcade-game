using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Input Action References")]
    [SerializeField] private InputActionReference _movementInputActionReference;
    [SerializeField] private InputActionReference _jumpInputActionReference;
    [SerializeField] private InputActionReference _dashInputActionReference;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 5f;
    private Vector2 _movementInputs; // Store the movement input values to be used in FixedUpdate
    public bool canMove = true; 

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 5f;
    [SerializeField] private GameObject _groundCheckPoint;
    private bool _isGrounded = true;
    public bool canJump = true;

    [Header("Dash Settings")]
    [SerializeField] private float _dashForce = 10f;
    public bool canDash = true;

    [Header("Gravity Settings")]
    private float _gravityValue = -9.81f;
    private float _originalGravityScale;

    [Header("Miscellaneous Settings")]
    [SerializeField] private LayerMask _walkableGroundLayerMask;
    private Rigidbody2D _playerRigidbody2D;


    private void Awake()
    {
        if (_movementInputActionReference == null && _jumpInputActionReference == null && _dashInputActionReference == null)
            throw new MissingReferenceException("Input Action References are not assigned in the inspector. Please assign them in the inspector.");

        if (!(TryGetComponent<Rigidbody2D>(out _playerRigidbody2D)))
        {
            throw new MissingComponentException("Rigidbody2D component is missing from the GameObject. Please add a Rigidbody2D component.");
        }
        else
        {
            _originalGravityScale = _playerRigidbody2D.gravityScale;
        }
    }

    private void OnEnable()
    {
        _movementInputActionReference.action.Enable();
        _jumpInputActionReference.action.Enable();
        _dashInputActionReference.action.Enable();
    }

    private void OnDisable()
    {
        _movementInputActionReference.action.Disable();
        _jumpInputActionReference.action.Disable();
        _dashInputActionReference.action.Disable();
    }

    private void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.transform.position, 0.1f, _walkableGroundLayerMask);

        if (_movementInputActionReference != null)
            _movementInputs = _movementInputActionReference.action.ReadValue<Vector2>();

        if (_movementInputs.Equals(Vector2.zero) && _isGrounded)
        {
            _playerRigidbody2D.gravityScale = 0f;
        }
        else
        {
            _playerRigidbody2D.gravityScale = _originalGravityScale;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = new Vector2(_movementInputs.x * _movementSpeed, _playerRigidbody2D.linearVelocity.y);
        _playerRigidbody2D.linearVelocity = AdjustedVelocityToSlope(velocity);

        if (_jumpInputActionReference.action.triggered && _isGrounded && canJump)
        {
            _playerRigidbody2D.linearVelocityY = Mathf.Sqrt(_jumpHeight * -2f * _gravityValue);
        }
    }

    private Vector2 AdjustedVelocityToSlope(Vector2 velocity)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1f, _walkableGroundLayerMask);

        if (hitInfo.collider != null)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector2.up, hitInfo.normal);
            Vector2 adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        return velocity;
    }
}