using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private int _currentTargetIndex = 0;
    private OldPlayerController _oldPlayerController;
    private Rigidbody2D _platformRigidBody;
    private Vector2 _previousPosition;
    public float Speed = 2f;
    public List<Transform> Waypoints;

    private void Start()
    {
	if (SceneHelper.TryFindGameObjectWithTagInScene("Player",out var playerGameObject))
            _oldPlayerController = playerGameObject.GetComponent<OldPlayerController>();

        _platformRigidBody = GetComponent<Rigidbody2D>();
        _previousPosition = _platformRigidBody.position;

        if (Waypoints.Count == 0)
            Debug.LogWarning("No Waypoints set for MovingTile.");
    }

    private void FixedUpdate()
    {
        if (Waypoints.Count == 0)
	    return;

        Vector2 currentPosition = _platformRigidBody.position;
        Vector2 targetPosition = Waypoints[_currentTargetIndex].position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, Speed * Time.fixedDeltaTime);
        _platformRigidBody.MovePosition(newPosition);

        // This doesn't effect the movement on the platform but does effect the player
        _platformRigidBody.linearVelocity = (newPosition - _previousPosition) / Time.fixedDeltaTime;
        _previousPosition = newPosition;

        if (Vector2.Distance(newPosition, targetPosition) < 0.05f)
            _currentTargetIndex = (_currentTargetIndex + 1) % Waypoints.Count;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _oldPlayerController != null)
            _oldPlayerController.MovingTileRigidbody = _platformRigidBody;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _oldPlayerController != null)
            _oldPlayerController.MovingTileRigidbody = null;
    }

    private void OnDrawGizmos()
    {
        if (Waypoints == null || Waypoints.Count < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < Waypoints.Count; i++)
        {
            Vector3 current = Waypoints[i].position;
            Vector3 next = Waypoints[(i + 1) % Waypoints.Count].position;
            Gizmos.DrawLine(current, next);
        }
    }
}
