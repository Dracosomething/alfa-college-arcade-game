using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class FlyingEnemy : Enemy
{
    private Vector3 _targetPosition;
    private NavMeshAgent _agent;
    private bool _hasSeenPlayerRecently;
    private Vector3 _spawnPosition;
    private Coroutine _wanderCoroutine;
    private Coroutine _forgetCoroutine;
    [SerializeField] private float _wanderRadius = 10f;
    [SerializeField] private float _wanderInterval = 3f;
    [SerializeField] private float _forgetDelay = 10f;
    [SerializeField] private float _sightDistance = 10f;

    public override void Awake()
    {
        base.Awake();
        
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _spawnPosition = transform.position;

        if (_wanderCoroutine == null)
            _wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    public override void Update()
    {
        base.Update();
        CheckIfPlayerCanBeSeen();
    }

    private void FixedUpdate() =>
        Move();

    private void CheckIfPlayerCanBeSeen()
    {
        Vector3 currentEnemyPosition = transform.position;
        var isPlayerVisible = false;
        
        Vector2 directionToPlayer = (Player.transform.position - currentEnemyPosition).normalized;
        RaycastHit2D[] allObjectsHitByRay = Physics2D.RaycastAll(currentEnemyPosition, directionToPlayer, _sightDistance);
        Debug.DrawLine(currentEnemyPosition, currentEnemyPosition + (Vector3)directionToPlayer * _sightDistance);

        if (allObjectsHitByRay.Length != 0)
        {
            isPlayerVisible = allObjectsHitByRay
                .Where(raycastHitObjectInfo => (bool)raycastHitObjectInfo.collider) // Filters out all RaycastHit2D where the collider is null
                .Where(raycastHitObjectInfo => (bool)raycastHitObjectInfo.collider.gameObject) // Filters out all RaycastHit2D where collider.gameObject is null
                .Any(raycastHitObjectInfo => raycastHitObjectInfo.collider.CompareTag("Player")); // Checks if any of the collided objects are the player.
        }

        if (isPlayerVisible)
        {
            _targetPosition = Player.transform.position;
            _hasSeenPlayerRecently = true;

            if (_forgetCoroutine != null)
            {
                StopCoroutine(_forgetCoroutine);
                _forgetCoroutine = null;
            }
        }
        else
        {
            if (_hasSeenPlayerRecently && _forgetCoroutine == null)
            {
                _forgetCoroutine = StartCoroutine(ForgetPlayer());
            }
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!_hasSeenPlayerRecently)
            {
                Vector2 randomPointInWanderRadius = Random.insideUnitCircle * _wanderRadius;
                Vector3 directionToWonderTo = _spawnPosition + new Vector3(randomPointInWanderRadius.x, randomPointInWanderRadius.y, _spawnPosition.z);

                // If we can find a mesh we can move to.
                if (NavMesh.SamplePosition(sourcePosition: directionToWonderTo, out NavMeshHit foundMesh, _wanderRadius, NavMesh.AllAreas))
                {
                    _targetPosition = new Vector3(foundMesh.position.x, foundMesh.position.y, _spawnPosition.z);
                }
                else
                {
                    _targetPosition = new Vector3(directionToWonderTo.x, directionToWonderTo.y, _spawnPosition.z);
                }
            }

            yield return new WaitForSeconds(_wanderInterval);
        }
    }

    private IEnumerator ForgetPlayer()
    {
        yield return new WaitForSeconds(_forgetDelay);
        _hasSeenPlayerRecently = false;
        _forgetCoroutine = null;
    }

    private void Move()
    {
        _agent.SetDestination(_targetPosition);
    }
}
