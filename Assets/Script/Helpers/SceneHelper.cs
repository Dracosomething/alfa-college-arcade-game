using UnityEngine;

public class SceneHelper
{
    public static bool TryFindFirstObjectByTypeInScene<T>(out T foundObject) where T : Object
    {
        foundObject = Object.FindFirstObjectByType<T>();
        return (object) foundObject != null;
    }
}