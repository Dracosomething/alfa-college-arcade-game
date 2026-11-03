using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    [Component] private PlayerCheckpointController _playerCheckpointController;
    [Component] private PlayerAnimator _playerAnimator;
    private bool _isPlayerAlreadyDying = false;

    [field: SerializeField] public int MaximumLives { get; private set; } = 3;
    public int CurrentLives { get; private set; }

    private void Start() =>
        CurrentLives = MaximumLives;

    public void Die()
    {
        const float TakingDamageAnimationTransitionLength = 0.25f;

        if (_isPlayerAlreadyDying)
            return;

        _isPlayerAlreadyDying = true;

        CurrentLives--;

        _playerAnimator.SetAnimationState(PlayerAnimationState.TookDamage);

        Invoke("RespawnPlayer", TakingDamageAnimationTransitionLength);
    }

    private bool IsDead() =>
        CurrentLives <= 0;

    private void RespawnPlayer()
    {
        _isPlayerAlreadyDying = false;

        if (IsDead())
        {
            _playerCheckpointController.ResetLevel();
            return;
        }

        _playerCheckpointController.GoToLastCheckpoint();
    }
}
