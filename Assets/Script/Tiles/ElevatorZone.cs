using System;
using UnityEngine;

public class ElevatorZone : MonoBehaviour
{
    [SerializeField] private int _zoneId;
    [SerializeField] private ElevatorPlatform _elevatorPlatform;

    private void OnTriggerEnter2D(Collider2D collidedObject)
    {
        if (collidedObject.CompareTag("Player"))
            _elevatorPlatform.OnPlayerEnteredZone(_zoneId, collidedObject.transform);
    }
}
