using UnityEngine;
using System;

public class BouncingPlatform : MonoBehaviour
{
    private Rigidbody2D _playerRigidBody;

    public float BounceForce;

    public void Awake()
    {
	if (!SceneHelper.TryFindGameObjectWithTagInScene("Player", out var player))
		throw new Exception("Object with tag \"Player\" not found.");
        _playerRigidBody = player.GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _playerRigidBody.AddForce(new Vector2(0, BounceForce * 10), ForceMode2D.Impulse);
    }
}
