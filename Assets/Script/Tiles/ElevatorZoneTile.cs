using System;
using UnityEngine;

public class ElevatorZone : MonoBehaviour
{
    [SerializeField] private int _zoneId;
    [SerializeField] private ElevatorPlatformTile elevatorPlatformTile;

    private void OnTriggerEnter2D(Collider2D collidedObject)
    {
        if (collidedObject.CompareTag("Player"))
            elevatorPlatformTile.OnPlayerEnteredZone(_zoneId, collidedObject.transform);
    }
}
