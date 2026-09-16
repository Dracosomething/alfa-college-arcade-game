using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private CustomTimeSpan _attackCooldown = new(minutes: 0, seconds: 0);
    [SerializeField] protected int _damage = 1;

    private long _remainingCooldownTimeInSeconds;
    private Vector2 _previousPosition;
    private SpriteRenderer _spriteRenderer;
    protected GameObject Player;
    protected Health PlayerHealthComponent;
    protected Rigidbody2D RigidBody;
    [SerializeField] protected int Health = 1;
    [SerializeField] protected float Speed = 1;

    protected virtual void Awake()
    {
        if (!SceneHelper.TryFindGameObjectInScene(Constants.PlayerGameObjectName, out Player))
            throw new CouldNotFindGameObjectException(Constants.PlayerGameObjectName);
        
        if (!(TryGetComponent<Rigidbody2D>(out RigidBody) &&
              TryGetComponent<SpriteRenderer>(out _spriteRenderer) &&
              Player.TryGetComponent<Health>(out PlayerHealthComponent)))
            throw new MissingComponentException("GameObject Enemy is missing one of the following components:" +
                                                "RigidBody2D, SpriteRenderer, Health");
        
        _previousPosition = RigidBody.position;
    }

    protected virtual void Update()
    {
        if (Health <= 0) 
            Destroy(gameObject);
        
        ChangeRotationToMatchMovingDirection();
    }

    protected abstract void Move();
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsCooldownActive())
            return;

        var collidedObject = collision.gameObject;
        
        if (collidedObject.CompareTag(Constants.PlayerGameObjectName)) 
            DealDamageToPlayer(collidedObject);
    }

    private void ChangeRotationToMatchMovingDirection()
    {
        Vector2 currentPosition = RigidBody.position;
        
        if (currentPosition.x > _previousPosition.x)
            _spriteRenderer.flipX = false;
        else if (currentPosition.x < _previousPosition.x)
            _spriteRenderer.flipX = true;
        
        _previousPosition = currentPosition;
    }

    private void DealDamageToPlayer(GameObject player)
    {
        PlayerHealthComponent.TakeDamage(_damage, transform);
        
        StartAttackCooldown();
    }

    private void DecreaseCooldownEverySecond()
    {
        if (!IsCooldownActive())
            RemoveAttackCooldown();
        
        _remainingCooldownTimeInSeconds--;
    }

    private bool IsCooldownActive() =>
        _remainingCooldownTimeInSeconds > 0;

    private void StartAttackCooldown()
    {
        const float IntervalRepeatingOffsetInSeconds = 0f;
        const float IntervalInSeconds = 1f;
       
        _remainingCooldownTimeInSeconds = _attackCooldown.TimeInSeconds;
        
        InvokeRepeating(nameof(DecreaseCooldownEverySecond), IntervalRepeatingOffsetInSeconds, IntervalInSeconds);
    }

    private void RemoveAttackCooldown() =>
        CancelInvoke(nameof(DecreaseCooldownEverySecond));
}
