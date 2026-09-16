using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float _wanderRadius = 1f;
    [SerializeField] private float _sightDistance = 1f;
    private bool _hasSeenPlayerRecently;
    [SerializeField] private Cooldown _newWanderPositionCooldown = new(minutes: 3);
    [SerializeField] private Cooldown _forgetDelay = new(minutes: 10);
    private NavMeshAgent _agent;
    private Coroutine _wanderCoroutine;
    private Coroutine _forgetCoroutine;
    private Transform _playerTransform;
    private Vector2 _spawnPosition;

    protected override void Awake()
    {
        base.Awake();
        
        if (!SceneHelper.TryFindGameObjectInScene(Constants.PlayerGameObjectName, out GameObject playerObject))
            throw new CouldNotFindGameObjectException(Constants.PlayerGameObjectName);

        _playerTransform = playerObject.transform;
        
        if (!TryGetComponent<NavMeshAgent>(out _agent))
            throw new MissingComponentException("FlyingEnemy is missing a NavMeshAgent component.");
        
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _spawnPosition = (Vector2)transform.position;

        // _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    protected override void Update()
    {
        base.Update();
        
        // CheckIfPlayerCanBeSeen();
    }

    protected override void Move()
    {
        // If player seen and forget delay active, stop
        
        Vector2 _targetPosition;
        
        if (CanSeePlayer())
        {
            _targetPosition = (Vector2)_playerTransform.position;
            // Start delay for forgetting
        }
        else
        {
            if (_newWanderPositionCooldown.IsCooldownActive())
                return;
             
            _targetPosition = GetNewWanderPosition();
            _newWanderPositionCooldown.StartCooldown();
        }
             
        _agent.SetDestination(_targetPosition);
    }

    private bool CanSeePlayer()
    {
        var selfPosition = (Vector2)transform.position;
        
        Vector2 directionToPlayer = ((Vector2)_playerTransform.position - selfPosition);
        RaycastHit2D objectHitByRaycast = Physics2D.Raycast(origin: selfPosition, directionToPlayer, _sightDistance);

        return (bool)objectHitByRaycast.collider &&
               objectHitByRaycast.collider.CompareTag(Constants.PlayerGameObjectName);
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
    
    // private void CheckIfPlayerCanBeSeen()
    // {
    //     Vector3 currentEnemyPosition = transform.position;
    //     var isPlayerVisible = false;
    //     
    //     var directionToPlayer = (Vector2)(Player.transform.position - currentEnemyPosition).normalized;
    //     RaycastHit2D[] allObjectsHitByRay = Physics2D.RaycastAll(currentEnemyPosition, directionToPlayer, _sightDistance);
    //     Debug.DrawLine(currentEnemyPosition, currentEnemyPosition + (Vector3)directionToPlayer * _sightDistance);
    //
    //     if (allObjectsHitByRay.Length != 0)
    //     {
    //         isPlayerVisible = allObjectsHitByRay
    //             .Where(raycastHitObjectInfo => (bool)raycastHitObjectInfo.collider) // Filters out all RaycastHit2D where the collider is null
    //             .Where(raycastHitObjectInfo => (bool)raycastHitObjectInfo.collider.gameObject) // Filters out all RaycastHit2D where collider.gameObject is null
    //             .Any(raycastHitObjectInfo => raycastHitObjectInfo.collider.CompareTag("Player")); // Checks if any of the collided objects are the player.
    //     }
    //
    //     if (isPlayerVisible)
    //     {
    //         _targetPosition = Player.transform.position;
    //         _hasSeenPlayerRecently = true;
    //
    //         if (_forgetCoroutine != null)
    //         {
    //             StopCoroutine(_forgetCoroutine);
    //             _forgetCoroutine = null;
    //         }
    //     }
    //     else if (_hasSeenPlayerRecently && _forgetCoroutine == null)
    //         _forgetCoroutine = StartCoroutine(ForgetPlayer());
    // }
    //
    // private IEnumerator WanderRoutine()
    // {
    //     while (true)
    //     {
    //         if (!_hasSeenPlayerRecently)
    //         {
    //             Vector2 randomPointInWanderRadius = Random.insideUnitCircle * _wanderRadius;
    //             Vector3 directionToWonderTo = _spawnPosition + new Vector3(randomPointInWanderRadius.x, randomPointInWanderRadius.y, _spawnPosition.z);
    //
    //             // If we can find a mesh we can move to.
    //             if (NavMesh.SamplePosition(sourcePosition: directionToWonderTo, out NavMeshHit foundMesh, _wanderRadius, NavMesh.AllAreas))
    //             {
    //                 _targetPosition = new Vector3(foundMesh.position.x, foundMesh.position.y, _spawnPosition.z);
    //             }
    //             else
    //             {
    //                 _targetPosition = new Vector3(directionToWonderTo.x, directionToWonderTo.y, _spawnPosition.z);
    //             }
    //         }
    //
    //         yield return new WaitForSeconds(_newWanderPositionCooldown);
    //     }
    // }
    //
    // private IEnumerator ForgetPlayer()
    // {
    //     yield return new WaitForSeconds(_forgetDelay);
    //     _hasSeenPlayerRecently = false;
    //     _forgetCoroutine = null;
    // }
}
