using System.Linq;
using UnityEngine;

public interface IAbilityObject
{
    /// <summary>
    /// Activates the ability of the game object.
    /// </summary>
    /// <returns>Returns true if activation successful and false if it didn't activate.</returns>
    public bool ActivateAbility();

    public static bool IsAbilityObject(GameObject gameObject) => 
        gameObject.GetComponents<MonoBehaviour>() // We get all Script components
            .Where(component => component != null) // We filter out all components that are null
            .Any(component => typeof(IAbilityObject).IsAssignableFrom(component.GetType())); // Check if component is of a type that implements IAbilityObject
}