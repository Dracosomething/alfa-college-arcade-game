using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactionInputReference;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private GameObject _interactionCheckObject;

    private void Awake()
    {
        if (_interactionInputReference == null)
            throw new SerializeFieldNotSetException("Missing Interaction InputActionReference");

        if (_interactionCheckObject == null)
            throw new SerializeFieldNotSetException("Missing InteractionCheck GameObject");
    }

    private void Update()
    {
        GameObject interactable = Physics2D.OverlapCircle(_interactionCheckObject.transform.position, _interactionRadius).gameObject;

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
