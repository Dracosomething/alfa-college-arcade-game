using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float _wanderRadius = 1f;
    [SerializeField] private float _sightDistance = 1f;
    private bool _hasSeenPlayerRecently;
    [SerializeField] private CustomTimeSpan _newWanderPositionCooldownDuration = new(seconds: 1);
    private CooldownWrapper _newWanderPositionCooldown;
    private NavMeshAgent _agent;
    private Coroutine _wanderCoroutine;
    private Coroutine _forgetCoroutine;
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
        
        // _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    protected override void Update()
    {
        base.Update();
        
        // CheckIfPlayerCanBeSeen();
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
