using UnityEngine;

public class BenchInteractible : MonoBehaviour
{
    public void SetBenchAsCheckpoint(
        OldPlayerController/* replace monobehaviour with player when player controller is finished */ player) =>
        player._subCheckPointPosition = transform.position;
}