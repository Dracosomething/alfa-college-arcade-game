using UnityEngine;
using UnityEngine.Serialization;

public abstract class Enemy : MonoBehaviour
{
    private Vector2 _previousPosition;
    private SpriteRenderer _spriteRenderer;
    protected GameObject Player;
    protected Rigidbody2D RigidBody;
    public int Health;
    public int Damage;
    public float Speed;

    public virtual void Awake()
    {
        Player = GameObject.Find("Player");
        RigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _previousPosition = RigidBody.position;
    }

    public virtual void Update()
    {
        if (Health <= 0) 
            Destroy(gameObject);
        
        ChangeRotationToMatchMovingDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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


    private void DealDamageToPlayer(GameObject player) => 
        player.GetComponent<Health>().TakeDamage(Damage, transform);
}
