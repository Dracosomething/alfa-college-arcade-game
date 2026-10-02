using System.Collections;
using System.Threading;
using UnityEngine;

public class HazardPlatformTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (!trigger.gameObject.CompareTag(Constants.PlayerGameObjectName))
            return;
        
        // Deal damage here when branches merged
    }
}
