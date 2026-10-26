using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactionInputReference;
    [SerializeField] private float _interactionRadius = 1;
    [SerializeField] private GameObject _interactionCheckObject;
    [SerializeField] private LayerMask _interactableObjectLayer;

    private void Awake()
    {
        if (_interactionInputReference.IsUnityNull())
            throw new SerializeFieldNotSetException("Missing Interaction InputActionReference");

        if (_interactionCheckObject.IsUnityNull())
            throw new SerializeFieldNotSetException("Missing InteractionCheck GameObject");
    }

    private void Update()
    {
        if (!_interactionInputReference.action.WasPressedThisFrame())
            return;

        Collider2D interactableObjectCollider = Physics2D.OverlapCircle(_interactionCheckObject.transform.position, _interactionRadius, _interactableObjectLayer);

        if (interactableObjectCollider.IsUnityNull())
            return;

        InteractWithObject(interactableObjectCollider.gameObject);
    }

    private void InteractWithObject(GameObject interactedObject)
    {
        if (interactedObject.TryGetComponent<IInteractionObject>(out IInteractionObject interactedObjectInteraction))
            interactedObjectInteraction.Interact();
    }
}
