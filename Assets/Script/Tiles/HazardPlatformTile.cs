using UnityEngine;

public class HazardPlatformTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        GameObject collidedObject = trigger.gameObject;
        
        if (!(collidedObject.CompareTag(Constants.PlayerGameObjectName) && 
              collidedObject.TryGetComponent<PlayerLifeController>(out var lifeController)))
            return;
        
        lifeController.Die();
    }
}
