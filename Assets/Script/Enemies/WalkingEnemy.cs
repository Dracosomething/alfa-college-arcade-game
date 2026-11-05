using System;
using System.ComponentModel;
using UnityEngine;

public class WalkingEnemy : Enemy
{
    [RequiredSerializeField] [SerializeField] private int _endPositionXOffset;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private Vector2 _endPosition;
    private TargetPosition _targetPositionOption = TargetPosition.EndPosition;
    
    protected override void Start()
    {
        base.Start();
        
        _startPosition = transform.position;
        _endPosition = new Vector2(_startPosition.x + _endPositionXOffset, _startPosition.y);
        _targetPosition = _endPosition;
    }

    protected override void Move()
    {
        const float TargetPositionEqualsAllowedOffset = 0.1f;
        
        RigidBody.position = Vector2.MoveTowards(
            current: RigidBody.position,
            target: _targetPosition,
            maxDistanceDelta: Speed * Time.fixedDeltaTime);

        if (Math.Abs(RigidBody.position.x - _targetPosition.x) < TargetPositionEqualsAllowedOffset)
            SwitchTargetPosition();
    }

    /// <summary>
    /// Method can't be single line since it returns nothing and handles more then one thing each case
    /// </summary>
    private void SwitchTargetPosition()
    {
        switch(_targetPositionOption)
        {
            case TargetPosition.StartPosition:
                _targetPositionOption = TargetPosition.EndPosition;
                _targetPosition = _endPosition;
                break;
            case TargetPosition.EndPosition:
                _targetPositionOption = TargetPosition.StartPosition;
                _targetPosition = _startPosition;
                break;
            default:
                throw new InvalidEnumArgumentException("TargetPosition enum only allows the values of '0'(StartPosition) and '1'(EndPosition)");
        }
    }
}
