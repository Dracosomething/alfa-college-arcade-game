using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AbilityActivationController), typeof(SoundEffectController))]
public class AbilityActivationController : MonoBehaviour, ICustomUnityScript
{
    [SerializeField] private InputActionReference _interactionInputActionReference;
    [Component] private AbilitySelectionController _selectionController;
    [Component] private SoundEffectController _soundEffectController;
    
    private void Awake()
    {
        ((ICustomUnityScript)this).InitializeAndValidateFields(this);
    }

    private void Update()
    {
        const string FailureAudioEffectName = "Concrete 1"; // Change this value when we have a proper sound effect for this.
        
        if (_interactionInputActionReference.action.WasPressedThisFrame() && !ActivateAbilityOfSelectedObject())
            _soundEffectController.PlaySound(FailureAudioEffectName, soundEffectOwnerName: name, allowMultipleOfSoundEffect: true);
    }

    private bool ActivateAbilityOfSelectedObject()
    {
        IAbilityObject? selectedObject = _selectionController.GetSelectedAbilityObject();

        if (selectedObject.IsUnityNull())
            return false;

        return selectedObject.ActivateAbility();
    }
}