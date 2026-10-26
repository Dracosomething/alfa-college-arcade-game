using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerAnimationStates _currentState;
    private Direction _currentDirection;
    private Animator _playerAnimator;

    private void Awake() =>
        // No need to get do a try catch here since the Animator is a required component.
        _playerAnimator = GetComponent<Animator>();

    public void GetValues(Vector2 movementInputs,  Vector2 velocity, bool isGrounded)
    {
        SetDirection(GetMovingDirection(movementInputs));

        SetAnimationState(GetAnimationState(movementInputs, velocity, isGrounded));
    }

    private Direction GetMovingDirection(Vector2 movementInputs)
    {
        const float NoInput = 0f;

        if (movementInputs.x > NoInput)
            return Direction.Right;
        else if (movementInputs.x < NoInput)
            return Direction.Left;

        return _currentDirection;
    }

    private PlayerAnimationStates GetAnimationState(Vector2 movementInputs, Vector2 velocity, bool isGrounded)
    {
        const float DashThreshold = 5f;
        const float JumpThreshold = 0.1f;
        const float NoInput = 0f;

        // Check for dashing first because that has priority over jumping, walking and idle.
        if ((velocity.x > DashThreshold || velocity.x < -DashThreshold) && velocity.y.Equals(NoInput))
            return PlayerAnimationStates.IsDashing;

        // Checking for jumping because that has priority over walking and idle.
        if (velocity.y > JumpThreshold && !isGrounded)
            return PlayerAnimationStates.IsJumping;
        else if (velocity.y < -JumpThreshold && !isGrounded)
            return PlayerAnimationStates.IsFalling;

        if (movementInputs.x != NoInput) // Either if x is positive or negative.
            return PlayerAnimationStates.IsWalking;

        // Return default animation state.
        return PlayerAnimationStates.IsIdle;
    }

    private void SetAnimationState(PlayerAnimationStates state)
    {
        if (_currentState.Equals(state))
            return;

        ResetStates();

        _currentState = state;
        _playerAnimator.SetBool(_currentState.ToString(), true);
    }

    private void SetDirection(Direction direction)
    {
        const string DirectionParameter = "IsFacingRight";

        if (_currentDirection.Equals(direction))
            return;

        _currentDirection = direction;
        _playerAnimator.SetBool(Animator.StringToHash(DirectionParameter), _currentDirection == Direction.Right);
    }

    private void ResetStates()
    {
        foreach (PlayerAnimationStates state in Enum.GetValues(typeof(PlayerAnimationStates)))
            _playerAnimator.SetBool(Animator.StringToHash(state.ToString()), false);
    }
}