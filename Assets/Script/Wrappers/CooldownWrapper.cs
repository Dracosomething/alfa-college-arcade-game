using System;
using UnityEngine;

public class CooldownWrapper : MonoBehaviour, IDynamicExceptionProvider
{
    private const int FinishedCooldownInSeconds = 0;
    
    [SerializeField] private CustomTimeSpan _cooldownDuration = new(minutes: 0, seconds: 0);
    private float _remainingCooldownInSeconds;

    private void Awake() =>
        ((IDynamicExceptionProvider)this).ValidateSerializeFieldsSet(this);

    public bool IsCooldownActive() =>
        _remainingCooldownInSeconds > FinishedCooldownInSeconds;

    public void StartCooldown()
    {
        if (IsCooldownActive())
            return;
        
        _remainingCooldownInSeconds = _cooldownDuration.TimeInSeconds;
    }

    private void Update() =>
        DecreaseCooldown();

    private void DecreaseCooldown()
    {
        if (!IsCooldownActive())
        {
            StopCooldown();
            return;
        }

        _remainingCooldownInSeconds -= Time.deltaTime;
    }
    
    private void StopCooldown() =>
        _remainingCooldownInSeconds = FinishedCooldownInSeconds;
}