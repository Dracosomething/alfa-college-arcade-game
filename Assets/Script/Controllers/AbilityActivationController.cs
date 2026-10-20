using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AbilityActivationController), typeof(SoundEffectController))]
public class AbilityActivationController : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactionInputActionReference;
    private AbilitySelectionController _selectionController;
    private SoundEffectController _soundEffectController;
    
    private void Awake()
    {
        if (_interactionInputActionReference == null)
            throw new SerializeFieldNotSetException("_interactionInputActionReference has not been set in de editor.");
        
        if (!(TryGetComponent<AbilitySelectionController>(out _selectionController) &&
              TryGetComponent<SoundEffectController>(out _soundEffectController)))
            throw new MissingComponentException("Missing an AbilityActivationController or SoundEffectController components!");
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