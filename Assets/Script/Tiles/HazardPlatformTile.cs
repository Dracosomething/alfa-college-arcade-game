using System.Collections;
using System.Threading;
using UnityEngine;

public class HazardPlatformTile : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject _damageEffectPrefab;
    [SerializeField] private float _damageEffectDuration = 0.2f;
    [SerializeField] private Vector2 _damageEffectOffset = new Vector2(0.3f, 0.2f);

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        OldPlayerController oldPlayerController = trigger.gameObject.GetComponent<OldPlayerController>();

        Vector3 collisionPosition = Vector3.Lerp(trigger.transform.position, this.transform.position, 0.5f);
        
        float facingDirection = 1f; 
        if (oldPlayerController != null)
        {
            Rigidbody2D playerRigidbody = trigger.gameObject.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
                facingDirection = playerRigidbody.linearVelocity.x < 0 ? -1f : 1f;
        }
                
        if (_damageEffectPrefab != null)
            StartCoroutine(SpawnDamageEffect(collisionPosition, facingDirection));
                
        StartCoroutine(WaitForKnockbackThenRespawn(oldPlayerController, trigger.gameObject));
    }

    public IEnumerator WaitForKnockbackThenRespawn(OldPlayerController oldPlayerController, GameObject player)
    {
        // Knockback knockback = player.GetComponent<Knockback>();

        // if (knockback != null)
        // {
        //     
        //     while (knockback.IsBeingKnockedBack)
        //     {
        //         yield return new WaitForFixedUpdate();
        //     }
        //     
        yield return new WaitForSeconds(0.1f);
        // }

        if (oldPlayerController != null)
            oldPlayerController.SubCheckpoints();
    }
    
    private IEnumerator SpawnDamageEffect(Vector3 position, float facingDirection)
    {
        Vector3 effectPosition = position + Vector3.up * _damageEffectOffset.y + Vector3.right * (_damageEffectOffset.x * facingDirection);
        
        GameObject effectInstance = Instantiate(_damageEffectPrefab, effectPosition, Quaternion.identity);
        
        yield return new WaitForSeconds(_damageEffectDuration);
        
        if (effectInstance != null)
            Destroy(effectInstance);
    }
}
