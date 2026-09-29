using System;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    private CheckpointController _checkpointController;

    [field: SerializeField] public int MaximumLives { get; private set; }
    public int CurrentLives { get; private set; }

    public void Die()
    {
        CurrentLives -= 1;

        if (CurrentLives == 0)
            _checkpointController.ResetLevel();

        _checkpointController.GoToLastCheckpoint();
    }

    private void Start()
    {
        if (!TryGetComponent<CheckpointController>(out _checkpointController))
            throw new MissingComponentException("No CheckpointController found on player.");

        CurrentLives = MaximumLives;
    }
}
