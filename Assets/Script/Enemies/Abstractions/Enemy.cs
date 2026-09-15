using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int Damage = 1;
    [SerializeField] private CustomTimeSpan AttackCooldown = new(minutes: 0, seconds: 0);

    private long _remainingCooldownTimeInSeconds;
    private Vector2 _previousPosition;
    private SpriteRenderer _spriteRenderer;
    protected GameObject Player;
    protected Rigidbody2D RigidBody;
    public int Health;
    public float Speed;

    public virtual void Awake()
    {
        if (!SceneHelper.TryFindGameObjectInScene(Constants.PlayerGameObjectName, out Player))
            throw new CouldNotFindGameObjectException(Constants.PlayerGameObjectName);
        
        if (!(TryGetComponent<Rigidbody2D>(out RigidBody) &&
              TryGetComponent<SpriteRenderer>(out _spriteRenderer)))
            throw new MissingComponentException("GameObject Enemy is missing one of the following components:" +
                                                "RigidBody2D, SpriteRenderer");
        
        _previousPosition = RigidBody.position;
    }

    public virtual void Update()
    {
        if (Health <= 0) 
            Destroy(gameObject);
        
        ChangeRotationToMatchMovingDirection();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // use Time.deltaTime to decrease cooldown every second
        if (_remainingCooldownTimeInSeconds > 0)
            return;
        
        if (collision.gameObject.CompareTag("Player")) 
            DealDamageToPlayer(collision.gameObject);
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
        player.GetComponent<Health>().TakeDamage(Damage, transform);
        _remainingCooldownTimeInSeconds = AttackCooldown.TimeInSeconds;
        // InvokeRepeating(nameof(DecreaseCooldownEverySecond), );
    }

    private void DecreaseCooldownEverySecond() =>
        _remainingCooldownTimeInSeconds--;
}
