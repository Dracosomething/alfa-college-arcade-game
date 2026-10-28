using UnityEngine;

public class CheckpointBenchTile : MonoBehaviour, IInteractionObject
{
    private PlayerCheckpointController _playerCheckpointController;
    
    private void Awake()
    {
        if (!SceneHelper.TryFindGameObjectInScene("Player", out GameObject playerGameObject))
            throw new CouldNotFindGameObjectException("Player");

        if (!playerGameObject.TryGetComponent<PlayerCheckpointController>(out _playerCheckpointController))
            throw new MissingComponentException("Missing Component CheckpointController on Player");
    }

    void IInteractionObject.Interact() =>
        _playerCheckpointController.SetCheckpoint(transform);
}
