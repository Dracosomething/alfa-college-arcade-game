using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private CustomTimeSpan _attackCooldownDuration = new(minutes: 0, seconds: 0);
    [SerializeField] protected int Damage = 1;

    private long _remainingCooldownTimeInSeconds;
    private Vector2 _previousPosition;
    private SpriteRenderer _spriteRenderer;
    private CooldownWrapper _attackCooldownWrapper;
    protected Rigidbody2D RigidBody;
    [SerializeField] protected int Health = 1;
    [SerializeField] protected float Speed = 1;

    protected virtual void Awake()
    {
        if (!(TryGetComponent<Rigidbody2D>(out RigidBody) &&
              TryGetComponent<SpriteRenderer>(out _spriteRenderer)))
            throw new MissingComponentException("GameObject Enemy is missing one of the following components:" +
                                                "RigidBody2D, SpriteRenderer");
        
        _previousPosition = RigidBody.position;
        _attackCooldownWrapper = this.AddComponent<CooldownWrapper>();
        _attackCooldownWrapper.InitializeCooldownWrapper(_attackCooldownDuration);
    }

    protected virtual void Update()
    {
        if (Health <= 0) 
            Destroy(gameObject);
        
        ChangeRotationToMatchMovingDirection();
    }

    protected abstract void Move();
    
    private void FixedUpdate() =>
        Move();
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_attackCooldownWrapper.IsCooldownActive())
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
        if (!player.TryGetComponent<Health>(out var playerHealth))
            throw new MissingComponentException("Gameobject Player is missing Health component!");
        
        playerHealth.TakeDamage(Damage, transform);
        
        _attackCooldownWrapper.StartCooldown();
    }
}
