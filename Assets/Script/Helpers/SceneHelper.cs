using UnityEngine;

public class SceneHelper
{
    public static bool TryFindFirstObjectByTypeInScene<T>(out T foundObject) where T : Object
    {
        foundObject = Object.FindFirstObjectByType<T>();
        return (object)foundObject != null;
    }

    public static bool TryFindGameObjectInScene(string objectName, out GameObject foundObject)
    {
        foundObject = GameObject.Find(objectName);
        return (object)foundObject != null;
    }
}