using UnityEngine;

public class PlayerAnimationManager
{
    private const string FacingDirectionAnimatorVariableName = "facingDirection";
    private const string SittingAnimatorVariableName = "isSitting";
    private const string WalkingAnimatorVariableName = "isWalking";
    private const string JumpingAnimatorVariableName = "isJumping";
    private const string FallingAnimatorVariableName = "isFalling";
    private const string ClimbingAnimatorVariableName = "isClimbing";
    private const string DashingAnimatorVariableName = "isDashing";
    
    
    private Animator _playerAnimator;

    public Direction FacingDirection
    {
        set => _playerAnimator.SetInteger(FacingDirectionAnimatorVariableName, (int)value);
        get => (Direction)_playerAnimator.GetInteger(FacingDirectionAnimatorVariableName);
    }

    public bool IsSitting
    {
        set => _playerAnimator.SetBool(SittingAnimatorVariableName, value);
        get => _playerAnimator.GetBool(SittingAnimatorVariableName);
    }

    public bool IsWalking
    {
        set => _playerAnimator.SetBool(WalkingAnimatorVariableName, value);
        get => _playerAnimator.GetBool(WalkingAnimatorVariableName);
    }

    public bool IsJumping
    {
        set => _playerAnimator.SetBool(JumpingAnimatorVariableName, value);
        get => _playerAnimator.GetBool(JumpingAnimatorVariableName);
    }

    public bool IsFalling
    {
        set => _playerAnimator.SetBool(FallingAnimatorVariableName, value);
        get => _playerAnimator.GetBool(FallingAnimatorVariableName);
    }

    public bool IsClimbing
    {
        set => _playerAnimator.SetBool(ClimbingAnimatorVariableName, value);
        get => _playerAnimator.GetBool(ClimbingAnimatorVariableName);
    }

    public bool IsDashing
    {
        set => _playerAnimator.SetBool(DashingAnimatorVariableName, value);
        get => _playerAnimator.GetBool(DashingAnimatorVariableName);
    }
    
    public PlayerAnimationManager(Animator playerAnimator)
    {
        _playerAnimator = playerAnimator;
    }
}