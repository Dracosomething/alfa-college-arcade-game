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

    public void SetAnimation(PlayerAnimationStates state, Direction direction, Vector2 velocity = default)
    {
        SetAnimationState(state, velocity);
        CheckMovingDirection(velocity);
    }

    private void SetAnimationState(PlayerAnimationStates state, Vector2 velocity = default)
    {
        ResetStates();

        if (_currentState == state)
            return;

        _currentState = state;
        _playerAnimater.SetBool(_currentState.ToString(), true);
    }
    private void CheckMovingDirection(Vector2 velocity)
    {
        if (velocity.x > 0)
            SetDirection(Direction.Right);
        else if (velocity.x < 0)
            SetDirection(Direction.Left);
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