using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AbilitySelectionController : MonoBehaviour
{
    [SerializeField] private AbilityActivationController _activationController;
    [SerializeField] private InputActionReference _scrollInputActionReference;
    private Transform _cameraTransform;
    private List<SpriteRenderer> _abilityObjectsInScene;
    private List<SpriteRenderer> _abilityObjectsInView;
    private SpriteRenderer? _selectedAbilityObject = null;
    private Vector2 _previousCameraPosition;
    
    private void Awake()
    {
        if (TryGetComponent<AbilityActivationController>(out _activationController))
            throw new MissingComponentException("Missing AbilityActionController component on self.");

        if (Camera.main == null)
            throw new NoMainCameraException();

        _cameraTransform = Camera.main.transform;
        _previousCameraPosition = (Vector2)_cameraTransform.position;
        
        Scene currentScene = SceneManager.GetActiveScene();
        _abilityObjectsInScene = GetAbilityObjectsInScene(currentScene).ToList();
    }

    private void Start() =>
        _abilityObjectsInView = GetAbilityObjectsInView().ToList();

    private void Update()
    {
        if ((Vector2)_cameraTransform.position != _previousCameraPosition)
            OnCameraChangePosition();
        
        if (_scrollInputActionReference.action.ReadValueAsInputState() == InputState.Pressed)
            ScrollThroughAbilityObjects();
    }

    private void OnCameraChangePosition()
    {
        _previousCameraPosition = (Vector2)_cameraTransform.position;

        _abilityObjectsInView = GetAbilityObjectsInView().ToList();
    }

    private void ScrollThroughAbilityObjects()
    {
        if (_selectedAbilityObject == null)
            _selectedAbilityObject = _abilityObjectsInView.First();
        else if (_selectedAbilityObject == _abilityObjectsInView.Last())
            _selectedAbilityObject = null;
        else
        {
            int selectedIndex = _abilityObjectsInView.IndexOf(_selectedAbilityObject);

            _selectedAbilityObject = _abilityObjectsInView[++selectedIndex];
        }
    }
    
    private IEnumerable<SpriteRenderer> GetAbilityObjectsInView() =>
        _abilityObjectsInScene
            .Where(renderer => renderer.isVisible); // We check if the renderer is visible
    
    private IEnumerable<SpriteRenderer> GetAbilityObjectsInScene(Scene currentScene) =>
        Resources.FindObjectsOfTypeAll<GameObject>() // We get all GameObjects currently loaded
            .Where(gameObject =>
                gameObject.scene == currentScene) // We filter out all GameObjects not in the current scene
            .Where(gameObject => gameObject.TryGetComponent<SpriteRenderer>(out var _)) // We filter out all GameObjects with no SpriteRenderer
            .Where(IAbilityObject.IsAbilityObject) // We filter out all GameObjects that are not an AbilityObject
            .Select(gameObject => gameObject.GetComponent<SpriteRenderer>()); // We select the SpriteRenderer
}