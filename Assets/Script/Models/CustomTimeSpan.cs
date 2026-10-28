using System;
using UnityEngine;

[Serializable]
public class CustomTimeSpan
{
    [field: SerializeField] public uint Minutes { get; private set; }
    [field: SerializeField] public uint Seconds { get; private set; }
    public long TimeInSeconds => (Minutes * 60) + Seconds;

    public CustomTimeSpan(uint minutes = 0, uint seconds = 0)
    {
        Minutes = minutes;
        Seconds = seconds;
    }
}