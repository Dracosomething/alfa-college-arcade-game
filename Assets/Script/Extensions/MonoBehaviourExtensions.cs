using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static bool TryFindFirstObjectByType<T>(this MonoBehaviour self, out T foundObject) where T : Object
    {
        foundObject = Object.FindFirstObjectByType<T>();
        return (object) foundObject != null;
    }
}