using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerAnimationState _currentState;
    private Direction _currentDirection;
    private Animator _playerAnimator;

    private void Awake()
    {
        if (!TryGetComponent<Animator>(out _playerAnimator))
            Debug.LogError("Animator component not found on Player.");
    }

    public void GetMovementAndGroundValues(Vector2 movementInputs,  Vector2 velocity, bool isGrounded)
    {
	const string SittingDownParameter = "IsSittingDown";

        SetDirection(GetMovingDirection(movementInputs));

        if (!movementInputs.Equals(Vector2.zero) || !velocity.Equals(Vector2.zero))
            SetPlayerAnimatorBool(SittingDownParameter, false);

        SetAnimationState(GetAnimationState(movementInputs, velocity, isGrounded));
    }

    public void SetAnimationState(PlayerAnimationState state)
    {
        if (_currentState.Equals(state))
            return;

        ResetStates();

        _currentState = state;
        SetPlayerAnimatorBool(_currentState.ToString(), true);
    }

    private Direction GetMovingDirection(Vector2 movementInputs)
    {
        if (movementInputs.x > Constants.NoInput)
            return Direction.Right;
        else if (movementInputs.x < Constants.NoInput)
            return Direction.Left;

        return _currentDirection;
    }

    private PlayerAnimationState GetAnimationState(Vector2 movementInputs, Vector2 velocity, bool isGrounded)
    {
        const float DashThreshold = 5f;
        const float JumpThreshold = 0.1f;

        // Check for dashing first because that has priority over jumping, walking and idle.
        if ((velocity.x > DashThreshold || velocity.x < -DashThreshold) && velocity.y.Equals(Constants.NoInput))
            return PlayerAnimationState.IsDashing;

        // Checking for jumping because that has priority over walking and idle.
        if (velocity.y > JumpThreshold && !isGrounded)
            return PlayerAnimationState.IsJumping;
        else if (velocity.y < -JumpThreshold && !isGrounded)
            return PlayerAnimationState.IsFalling;

        if (!movementInputs.x.Equals(Constants.NoInput)) // Either if x is positive or negative.
            return PlayerAnimationState.IsWalking;

        // Return default animation state.
        return PlayerAnimationState.IsIdle;
    }

    private void SetDirection(Direction direction)
    {
        const string DirectionParameter = "IsFacingRight";

        if (_currentDirection.Equals(direction))
            return;

        _currentDirection = direction;
        SetPlayerAnimatorBool(DirectionParameter, _currentDirection == Direction.Right);
    }

    private void ResetStates()
    {
        const string SittingDownParameter = "IsSittingDown";

        foreach (PlayerAnimationState state in Enum.GetValues(typeof(PlayerAnimationState)))
        {
            if (state.ToString().Equals(SittingDownParameter))
                continue;

            SetPlayerAnimatorBool(state.ToString(), false);
        }
    }

    private void SetPlayerAnimatorBool(string parameter, bool value) =>
        _playerAnimator.SetBool(Animator.StringToHash(parameter), value);
}
