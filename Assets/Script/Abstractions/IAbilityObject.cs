public interface IAbilityObject
{
    /// <summary>
    /// Activates the ability of the game object.
    /// </summary>
    /// <returns>Returns true if activation successful and false if it didn't activate.</returns>
    public bool ActivateAbility();

    /// <summary>
    /// Highlights the GameObject with a red boundary
    /// </summary>
    /// <returns>Returns true when successfull and false if the method failed.</returns>
    public bool HighlightObject();
}