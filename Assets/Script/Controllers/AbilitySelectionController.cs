using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AbilitySelectionController : MonoBehaviour
{
    [SerializeField] private InputActionReference _scrollInputActionReference;
    private Camera _mainCamera;
    private Transform _cameraTransform;
    private List<SpriteRenderer> _abilityObjectsInScene;
    private List<SpriteRenderer> _abilityObjectsInView;
    private SpriteRenderer? _selectedAbilityObject = null;
    private Vector2 _previousCameraPosition;
    
    private void Awake()
    {
        if (!TryGetComponent<InputActionReference>(out _scrollInputActionReference))
            throw new MissingComponentException("Missing AbilityActionController component on self.");

        if (Camera.main == null)
            throw new NoMainCameraException();

        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
        _previousCameraPosition = (Vector2)_cameraTransform.position;
        
        Scene currentScene = SceneManager.GetActiveScene();
        _abilityObjectsInScene = GetAbilityObjectsInScene(currentScene).ToList();

        _scrollInputActionReference.action.Enable();
    }

    private void Start() =>
        _abilityObjectsInView = GetAbilityObjectsInView().ToList();

    public IAbilityObject? GetSelectedAbilityObject()
    {
        if (_selectedAbilityObject.IsUnityNull() ||
            !_selectedAbilityObject.gameObject.TryGetComponent<IAbilityObject>(out var abilityObjectScript))
            return null;

        return abilityObjectScript;
    }
    
    private void Update()
    {
        if ((Vector2)_cameraTransform.position != _previousCameraPosition)
            OnCameraChangePosition();
        
        if (_scrollInputActionReference.action.WasPressedThisFrame())
        {
            UnHighlighSelected();
            
            ScrollThroughAbilityObjects();
            
            HighlightSelected();
        }
    }

    private void OnCameraChangePosition()
    {
        _previousCameraPosition = (Vector2)_cameraTransform.position;
        
        _abilityObjectsInView = GetAbilityObjectsInView().ToList();
        
        DeselectAbilityObjectIfNoLongerInView();
    }

    private void DeselectAbilityObjectIfNoLongerInView()
    {
        if (!_selectedAbilityObject.IsUnityNull() && !IsRendererVisible(_selectedAbilityObject))
        {
            UnHighlighSelected();
            _selectedAbilityObject = null;
        }
    }
    
    private void ScrollThroughAbilityObjects()
    {
        if (_selectedAbilityObject.IsUnityNull())
            _selectedAbilityObject = _abilityObjectsInView.First();
        else if (_selectedAbilityObject == _abilityObjectsInView.Last())
            _selectedAbilityObject = null;
        else
        {
            int selectedIndex = _abilityObjectsInView.IndexOf(_selectedAbilityObject);

            _selectedAbilityObject = _abilityObjectsInView[++selectedIndex];
        }
    }

    private void UnHighlighSelected()
    {
        if (_selectedAbilityObject.IsUnityNull())
            return;
        
        _selectedAbilityObject.material.color = Color.white;
    }
    
    private void HighlightSelected()
    {
        if (_selectedAbilityObject.IsUnityNull())
            return;
        
        _selectedAbilityObject.material.color = Color.red;
    }
    
    private IEnumerable<SpriteRenderer> GetAbilityObjectsInView() =>
        _abilityObjectsInScene
            .Where(IsRendererVisible); // We check if the renderer is visible
    
    private IEnumerable<SpriteRenderer> GetAbilityObjectsInScene(Scene currentScene) =>
        Resources.FindObjectsOfTypeAll<GameObject>() // We get all GameObjects currently loaded
            .Where(gameObject => gameObject.scene == currentScene) // We filter out all GameObjects not in the current scene
            .Where(gameObject => gameObject.TryGetComponent<SpriteRenderer>(out var _)) // We filter out all GameObjects with no SpriteRenderer
            .Where(IAbilityObject.IsAbilityObject) // We filter out all GameObjects that are not an AbilityObject
            .Select(gameObject => gameObject.GetComponent<SpriteRenderer>()); // We select the SpriteRenderer

    private bool IsRendererVisible(Renderer abilityObjectRenderer)
    {
        Vector2 viewPosition = _mainCamera.WorldToViewportPoint(abilityObjectRenderer.transform.position); // returns a vector2 where x and y are 0-1 if the position is in the camera

        return viewPosition.x is >= 0 and <= 1 && viewPosition.y is >= 0 and <= 1;
    }
}