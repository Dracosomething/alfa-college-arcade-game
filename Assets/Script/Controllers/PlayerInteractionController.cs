using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactionInputReference;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private GameObject _interactionObject;

    private void Awake()
    {
        if (_interactionInputReference == null)
            throw new MissingComponentException("Missing Interaction InputActionReference");
    }

    private void Update()
    {
        GameObject interactable = Physics2D.OverlapCircle(_interactionObject.transform.position, _interactionRadius).gameObject;

	if (interactable == null)
	    return;

	if (!_interactionInputReference.action.WasPressedThisFrame())
	    return;

        InteractWithObject(interactable);
    }

    private void InteractWithObject(GameObject interactedObject)
    {
        IInteractionObject interactedObjectInteraction = interactedObject.GetComponent<IInteractionObject>();
	interactedObjectInteraction.Interact();
    }
}
