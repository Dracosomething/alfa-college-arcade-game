using UnityEngine;

public class CheckpointBenchTile : MonoBehaviour, IInteractionObject
{
    private GameObject _playerGameObject;
    private PlayerCheckpointController _playerCheckpointController;
    private PlayerAnimator _playerAnimator;
    private Rigidbody2D _playerRigidbody;
    private bool _isPlayerRigidbodyFrozen = false;
    
    private void Awake()
    {
        if (!SceneHelper.TryFindGameObjectInScene("Player", out _playerGameObject))
            throw new CouldNotFindGameObjectException("Player");

        if (!_playerGameObject.TryGetComponent<PlayerCheckpointController>(out _playerCheckpointController))
            throw new MissingComponentException("Missing Component CheckpointController on Player");

        if (!_playerGameObject.TryGetComponent<PlayerAnimator>(out _playerAnimator))
            throw new MissingComponentException("Missing Component PlayerAnimator on Player");

        if (!_playerGameObject.TryGetComponent<Rigidbody2D>(out _playerRigidbody))
            throw new MissingComponentException("Missing Component PlayerRigidbody on Player");
    }

    void IInteractionObject.Interact()
    {
        float sittingDownAnimationTransitionLength = 0.25f; // Use a constant instead of getting the animator transition length because to my knowledge it is impossible to easily get

        // Lossy Scale is used because it grabs the global scale of the object instead of the local one, it should usually not give any problems in any case
        float playerLossyYScaleInHalf = _playerGameObject.transform.lossyScale.y / 2; // Use half because the Transform position is not in the middle

        Vector3 newCheckpointLocation = new Vector3(transform.position.x, transform.position.y + playerLossyYScaleInHalf, transform.position.z);

        _playerCheckpointController.SetCheckpoint(newCheckpointLocation);

	_playerAnimator.SetAnimationState(PlayerAnimationState.IsSittingDown);

        _playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

	_playerGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + playerLossyYScaleInHalf, transform.position.z);

        if (!_isPlayerRigidbodyFrozen)
	{
            _isPlayerRigidbodyFrozen = true;
            Invoke("UnfreezePlayer", sittingDownAnimationTransitionLength);
	}
    }

    private void UnfreezePlayer()
    {
        _playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _isPlayerRigidbodyFrozen = false;
    }
}
