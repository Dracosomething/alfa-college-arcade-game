using System.Collections;
using System.Threading;
using UnityEngine;

public class hazardDamage : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject _damageEffectPrefab;
    [SerializeField] private float _spawnDamageEffectDuration = 0.2f;
    [SerializeField] private Vector2 _spawnDamageEffectOffset = new Vector2(0.3f, 0.2f);

    [Header("")]
    public int damageAmount { get; private set; }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        Health health = trigger.gameObject.GetComponent<Health>();
        OldPlayerController oldPlayerController = trigger.gameObject.GetComponent<OldPlayerController>();

        if (health == null)
            return;

        if (health.isInvincibleStatus() != false)
            return;
        
        Vector3 collisionPosition = Vector3.Lerp(trigger.transform.position, this.transform.position, 0.5f);
        
        float facingDirection = 1f; 
        if (oldPlayerController != null)
        {
            // Access the horizontalMovement to determine facing direction
            // If player is moving left (negative), face left. If moving right (positive) or not moving, face right
            Rigidbody2D playerRb = trigger.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
                facingDirection = playerRb.linearVelocity.x < 0 ? -1f : 1f;
        }
                
        // Instantiate damage effect at collision position with facing direction
        if (_damageEffectPrefab != null)
            StartCoroutine(SpawnDamageEffect(collisionPosition, facingDirection));
                
        health.TakeDamage(damageAmount, this.transform, true); // true = play timeline animation
                
                // Start coroutine to wait for knockback to finish before respawning
        StartCoroutine(WaitForKnockbackThenRespawn(oldPlayerController, trigger.gameObject));
    }

    public IEnumerator WaitForKnockbackThenRespawn(OldPlayerController oldPlayerController, GameObject player)
    {
        // Get the knockback component to monitor its state
        Knockback knockback = player.GetComponent<Knockback>();
        Health health = player.GetComponent<Health>();
        
        if (knockback != null)
        {
            
            while (knockback.IsBeingKnockedBack)
            {
                yield return new WaitForFixedUpdate();
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        // Check if the player died during the damage - if so, don't respawn to last grounded
        if (health != null && health.IsDeadOrRespawning())
            yield break; // Exit the coroutine without calling LastGroundedRespawn
        
        // Now call the respawn after knockback is finished (only if player didn't die)
        if (oldPlayerController != null)
            oldPlayerController.SubCheckpoints();
    }
    
    private IEnumerator SpawnDamageEffect(Vector3 position, float facingDirection)
    {
        // Apply Y offset to move the effect slightly upward
        // Apply X offset based on facing direction (negative for left, positive for right)
        Vector3 effectPosition = position + Vector3.up * _spawnDamageEffectOffset.y + Vector3.right * (_spawnDamageEffectOffset.x * facingDirection);
        
        // Instantiate the damage effect prefab at the offset position
        GameObject effectInstance = Instantiate(_damageEffectPrefab, effectPosition, Quaternion.identity);
        
        // Wait for the specified duration
        yield return new WaitForSeconds(_spawnDamageEffectDuration);
        
        // Destroy the effect
        if (effectInstance != null)
        {
            Destroy(effectInstance);
        }
    }
}
