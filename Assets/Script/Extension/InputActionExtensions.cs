using UnityEngine.InputSystem;

public static class InputActionExtensions
{
    public static InputState ReadValueAsInputState(this InputAction self)
    {
        if (self.type != InputActionType.Button)
            return InputState.UnPressed;
        
        var value = self.ReadValue<float>();
        
        return (InputState)value;
    }
}