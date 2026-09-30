using UnityEngine;

public class WalkingEnemy : Enemy
{
    [SerializeField] private Vector2 _endPosition;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;

    protected override void Awake()
    {
        base.Awake();
        
        // Always first move towards end position.
        _startPosition = transform.position;
        _targetPosition = _endPosition;
    }

    protected override void Move()
    {
        RigidBody.position = Vector2.MoveTowards(
            current: RigidBody.position, 
            target: _targetPosition,
            maxDistanceDelta: Speed * Time.fixedDeltaTime);

        if (RigidBody.position == _targetPosition)
            SwitchTargetPosition();
    }

    private void SwitchTargetPosition()
    {
        if (_targetPosition == _endPosition)
            _targetPosition = _startPosition;
        else
            _targetPosition = _endPosition;
    }
}
