using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    [Component] private PlayerCheckpointController _playerCheckpointController;

    [field: SerializeField] public int MaximumLives { get; private set; } = 3;
    public int CurrentLives { get; private set; }

    private void Awake() =>
        CurrentLives = MaximumLives;

    public void Die()
    {
        CurrentLives--;

        if (IsDead())
        {
            _playerCheckpointController.ResetLevel();
            return;
        }

        _playerCheckpointController.GoToLastCheckpoint();
    }

    private bool IsDead() =>
        CurrentLives <= 0;
}
