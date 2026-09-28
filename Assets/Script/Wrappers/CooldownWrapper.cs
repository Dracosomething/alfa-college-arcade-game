using UnityEngine;

public class CooldownWrapper : MonoBehaviour
{
    private const float IntervalRepeatingOffsetInSeconds = 0.1f;
    private const float IntervalInSeconds = 1f; 
    
    private long _remainingCooldown;
    private CustomTimeSpan _timeSpan;
    
    public void InitializeCooldownWrapper(CustomTimeSpan timeSpan)
    {
        _timeSpan = timeSpan;
    }
    
    public bool IsCooldownActive() =>
        _remainingCooldown > 0;


    public void StartCooldown()
    {
        if (IsCooldownActive())
            return;
        
        _remainingCooldown = _timeSpan.TimeInSeconds;

        StartDecreasingCooldown();
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

    private void StartDecreasingCooldown() =>
        InvokeRepeating(nameof(DecreaseCooldownEverySecond), IntervalRepeatingOffsetInSeconds, IntervalInSeconds);
    
    private void StopCooldown()
    {
        CancelInvoke(nameof(DecreaseCooldownEverySecond));
        _remainingCooldown = 0;
    }
}