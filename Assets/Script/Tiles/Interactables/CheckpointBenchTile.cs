using UnityEngine;

public class CheckpointBenchTile : MonoBehaviour, IInteractionObject
{
    private GameObject _playerGameObject;
    private PlayerCheckpointController _playerCheckpointController;
    
    private void Awake()
    {
        if (!SceneHelper.TryFindGameObjectInScene("Player", out _playerGameObject))
            throw new CouldNotFindGameObjectException("Player");

        if (!_playerGameObject.TryGetComponent<PlayerCheckpointController>(out _playerCheckpointController))
            throw new MissingComponentException("Missing Component CheckpointController on Player");
    }

    void IInteractionObject.Interact()
    {
        // Lossy Scale is used because it grabs the global scale of the object instead of the local one, it should usually not give any problems in any case
        float playerLossyYScaleInHalf = _playerGameObject.transform.lossyScale.y / 2; // Use half because the Transform position is in the middle instead of in a corner
        Vector3 newCheckpointLocation = new Vector3(transform.position.x, transform.position.y + playerLossyYScaleInHalf, transform.position.z);

        _playerCheckpointController.SetCheckpoint(newCheckpointLocation);
    }
}
