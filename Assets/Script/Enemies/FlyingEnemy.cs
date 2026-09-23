using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float _wanderRadius = 1f;
    [SerializeField] private float _sightDistance = 1f;
    [SerializeField] private CustomTimeSpan _newWanderPositionCooldownDuration = new(seconds: 1);
    private CooldownWrapper _newWanderPositionCooldown;
    private NavMeshAgent _agent;
    private Vector2 _spawnPosition;

    protected override void Awake()
    {
        base.Awake();
        
        if (!TryGetComponent<NavMeshAgent>(out _agent))
            throw new MissingComponentException("FlyingEnemy is missing a NavMeshAgent component.");
        
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _spawnPosition = (Vector2)transform.position;

        _newWanderPositionCooldown = this.AddComponent<CooldownWrapper>();
        _newWanderPositionCooldown.InitializeCooldownWrapper(_newWanderPositionCooldownDuration);
    }

    protected override void Move()
    {
        if (CanSeePlayerAndGetPlayerPosition(out var playerPosition))
            _agent.SetDestination((Vector3)(playerPosition!));
        else
        {
            if (_newWanderPositionCooldown.IsCooldownActive())
                return;
            _agent.SetDestination((Vector3)GetNewWanderPosition());
            
            _newWanderPositionCooldown.StartCooldown();
        }
    }

    private bool CanSeePlayerAndGetPlayerPosition(out Vector2? playerPosition)
    {
        playerPosition = null;
        
        var selfPosition = (Vector2)transform.position;

        Collider2D[] collidingObjects = Physics2D.OverlapCircleAll(point: selfPosition, _sightDistance);
        Collider2D playerObject = collidingObjects
            .FirstOrDefault(collidedObject => collidedObject.CompareTag(Constants.PlayerGameObjectName));

        if (!playerObject)
            return false;

        playerPosition = playerObject.transform.position;
        
        return playerObject.CompareTag(Constants.PlayerGameObjectName);
    }

    private Vector2 GetNewWanderPosition()
    {
        Vector2 randomPointInWanderRadius = Random.insideUnitCircle * _wanderRadius;
        Vector2 positionToWanderTo = _spawnPosition + randomPointInWanderRadius;

        // We find the nearest point that we can move to from randomPointInWanderRadius
        if (NavMesh.SamplePosition(sourcePosition: positionToWanderTo, out NavMeshHit foundMesh, _wanderRadius,
                NavMesh.AllAreas))
            return (Vector2)foundMesh.position;
        
        return positionToWanderTo;
    }
}
