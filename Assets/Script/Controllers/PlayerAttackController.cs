using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Component] private Rigidbody2D _playerRigidbody;

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out var enemy) && 
            _playerRigidbody.position.y > other.transform.position.y)
            AttackEnemy(enemy);
    }

    private void AttackEnemy(Enemy enemy) =>
        enemy.Hurt();
}