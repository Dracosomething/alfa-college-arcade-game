using UnityEngine;

public class PlayerLifeController : MonoBehaviour, ICustomUnityScript
{
    [Component] private CheckpointController _checkpointController;

    [field: SerializeField] public int MaximumLives { get; private set; }
    public int CurrentLives { get; private set; }

    private void Awake()
    {
        ((ICustomUnityScript)this).InitializeAndValidateFields(this);

        CurrentLives = MaximumLives;
    }

    public void Die()
    {
        CurrentLives--;

        if (IsDead())
        {
            _checkpointController.ResetLevel();
            return;
        }

        _checkpointController.GoToLastCheckpoint();
    }

    private bool IsDead() =>
        CurrentLives <= 0;
}
