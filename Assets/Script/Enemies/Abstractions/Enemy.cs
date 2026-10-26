using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CooldownWrapper), typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
    [Component] private SpriteRenderer _spriteRenderer;
    [Component] private CooldownWrapper _attackCooldownWrapper;
    private Vector2 _previousPosition;
    private long _remainingCooldownTimeInSeconds;
    [Component] protected Rigidbody2D RigidBody;
    [SerializeField] protected int Health = 1;
    [SerializeField] protected float Speed = 1;

    protected virtual void Awake() =>
        _previousPosition = RigidBody.position;

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
        if (!player.TryGetComponent<PlayerLifeController>(out var lifeController))
            throw new MissingComponentException("Player is missing a LifeController component!");
            
        lifeController.Die();
        
        _attackCooldownWrapper.StartCooldown();
    }
}
