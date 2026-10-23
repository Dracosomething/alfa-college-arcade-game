using System.ComponentModel;
using UnityEngine;

public class WalkingEnemy : Enemy
{
    [SerializeField] private Vector2 _endPosition;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private TargetPosition _targetPositionOption = TargetPosition.EndPosition;
    
    protected override void Awake()
    {
        base.Awake();
        
        ((IDynamicExceptionProvider)this).ValidateSerializeFieldsSet(this);
        
        _startPosition = transform.position;
        _targetPosition = GetTargetPosition();
    }

    protected override void Move()
    {
        RigidBody.position = Vector2.MoveTowards(
            current: RigidBody.position,
            target: _targetPosition,
            maxDistanceDelta: Speed * Time.fixedDeltaTime);

        if (RigidBody.position.Equals(_targetPosition))
            SwitchTargetPosition();
    }

    private void SwitchTargetPosition()
    {
        switch(_targetPositionOption)
        {
            case TargetPosition.StartPosition:
                _targetPositionOption = TargetPosition.EndPosition;
                break;
            case TargetPosition.EndPosition:
                _targetPositionOption = TargetPosition.StartPosition;
                break;
            default:
                throw new InvalidEnumArgumentException("TargetPosition enum only allows the values of '0'(StartPosition) and '1'(EndPosition)");
        }

        _targetPosition = GetTargetPosition();
    }

    private Vector2 GetTargetPosition() =>
        _targetPositionOption switch
        {
            TargetPosition.StartPosition => _startPosition,
            TargetPosition.EndPosition => _endPosition,
            /* Default */ _ => throw new InvalidEnumArgumentException("TargetPosition enum only allows the values of '0'(StartPosition) and '1'(EndPosition)")
        };
}
