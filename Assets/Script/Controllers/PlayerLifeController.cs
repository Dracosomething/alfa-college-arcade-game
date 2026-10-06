using System;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    private CheckpointController _checkpointController;
    [SerializeField] private int _maximumLives;

    public int CurrentLives { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent<CheckpointController>(out _checkpointController))
            throw new MissingComponentException("No CheckpointController found on player.");

        CurrentLives = _maximumLives;
    }

    public void Die()
    {
        CurrentLives--;

        if (CurrentLives <= 0)
        {
            _checkpointController.ResetLevel();
            return;
        }

        _checkpointController.GoToLastCheckpoint();
    }
}
