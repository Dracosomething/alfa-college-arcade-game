using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    #region fields
    private PlayerAnimationStates _currentState;
    private Direction _currentDirection;
    private Animator _playerAnimater;
    #endregion

    private void Awake() =>
        // No need to get do a try catch here since the Animator is a required component.
        _playerAnimater = GetComponent<Animator>();

    public void GetValues(Vector2 movemenInputs,  Vector2 velocity, bool isGrounded)
    {
        SetDirection(GetMovingDirection(movemenInputs));

        SetAnimationState(GetAnimationState(movemenInputs, velocity, isGrounded));
    }

    private Direction GetMovingDirection(Vector2 movemenInputs)
    {
        if (movemenInputs.x > 0)
            return Direction.Right;
        else if (movemenInputs.x < 0)
            return Direction.Left;
        else
            return _currentDirection;
    }

    private PlayerAnimationStates GetAnimationState(Vector2 movemenInputs, Vector2 velocity, bool isGrounded)
    {
        // Check for dashing first because that has priority over jumping, walking and idle.
        if ((velocity.x > 5f || velocity.x < -5f) && velocity.y == 0f)
            return PlayerAnimationStates.IsDashing;

        // Checking for jumping because that has priority over walking and idle.
        if (velocity.y > 0.1f && !isGrounded)
            return PlayerAnimationStates.IsJumping;
        else if (velocity.y < -0.1f && !isGrounded)
            return PlayerAnimationStates.IsFalling;

        if (movemenInputs.x != 0) // Either if x is positive or negative.
            return PlayerAnimationStates.IsWalking;

        // Return default animation state.
        return PlayerAnimationStates.IsIdle;
    }

    private void SetAnimationState(PlayerAnimationStates state)
    {
        if (_currentState == state)
            return;

        ResetStates();

        _currentState = state;
        _playerAnimater.SetBool(_currentState.ToString(), true);
    }

    private void SetDirection(Direction direction)
    {
        if (_currentDirection == direction)
            return;

        _currentDirection = direction;
        _playerAnimater.SetBool(Animator.StringToHash("IsFacingRight"), _currentDirection == Direction.Right);
    }

    private void ResetStates()
    {
        foreach (PlayerAnimationStates state in Enum.GetValues(typeof(PlayerAnimationStates)))
        {
            _playerAnimater.SetBool(Animator.StringToHash(state.ToString()), false);
        }
    }
}