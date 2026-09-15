using System;
using UnityEngine;

[Serializable]
public class CustomTimeSpan
{
    // Gives the SerializeField to the underlying field
    [field: SerializeField] public int Minutes { get; private set; }
    [field: SerializeField] public int Seconds { get; private set; }
    public long TimeInSeconds => (Minutes * 60) + Seconds;
    
    public CustomTimeSpan(int minutes = 0, int seconds = 0)
    {
        Minutes = minutes;
        Seconds = seconds;
    }
}