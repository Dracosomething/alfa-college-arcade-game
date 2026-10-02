using UnityEngine;
using System;

public class BouncingPlatformTile : MonoBehaviour
{
    private const int BounceForceMultiplier = 10;
    
    [SerializeField] private float _bounceForce;
    private Rigidbody2D _playerRigidbody;
    
    public void Awake()
    {
	    if (!SceneHelper.TryFindGameObjectWithTagInScene(Constants.PlayerGameObjectName, out var player))
		    throw new CouldNotFindGameObjectException("Object with tag \"Player\" not found.");

        if (!player.TryGetComponent<Rigidbody2D>(out _playerRigidbody))
            throw new MissingComponentException("Player is missing a Rigidbody2D component.");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Constants.PlayerGameObjectName))
            _playerRigidbody.AddForce(new Vector2(Constants.NoMovement, _bounceForce * BounceForceMultiplier), ForceMode2D.Impulse);
    }
}
