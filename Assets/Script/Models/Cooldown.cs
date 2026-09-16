using System;
using UnityEngine;

[Serializable]
public class Cooldown : MonoBehaviour
{
   private const float IntervalRepeatingOffsetInSeconds = 0f;
   private const float IntervalInSeconds = 1f; 
    
    private long _remainingCooldown;
    
    // Gives the SerializeField to the underlying field
    [field: SerializeField] public int Minutes { get; private set; }
    [field: SerializeField] public int Seconds { get; private set; }
    public long TimeInSeconds => (Minutes * 60) + Seconds;
    
    public Cooldown(int minutes = 0, int seconds = 0)
    {
        Minutes = minutes;
        Seconds = seconds;
    }

    public bool IsCooldownActive() =>
        _remainingCooldown > 0;


    public void StartCooldown()
    {
        if (IsCooldownActive())
            return;
        
        _remainingCooldown = TimeInSeconds;
        
        InvokeRepeating(nameof(DecreaseCooldownEverySecond), IntervalRepeatingOffsetInSeconds, IntervalInSeconds);
    }
    
    private void DecreaseCooldownEverySecond()
    {
        if (!IsCooldownActive())
        {
            StopCooldown();
            return;
        }

        _remainingCooldown--;
    }

    private void StopCooldown()
    {
        CancelInvoke(nameof(DecreaseCooldownEverySecond));
        _remainingCooldown = 0;
    }
}