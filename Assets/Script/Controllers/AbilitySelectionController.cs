using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AbilitySelectionController : MonoBehaviour
{
    private const float DiagonalBoxCastAngle = 45f;
    private const float HorizontalBoxCastAngle = 0f;
    
    [Component] private PlayerMovementController _playerMovementController;
    [RequiredSerializeField, SerializeField] private InputActionReference _startScrollingInputActionReference;
    [RequiredSerializeField, SerializeField] private InputActionReference _scrollInputActionReference;
    private Camera _mainCamera;
    private Transform _cameraTransform;
    private List<SpriteRenderer> _abilityObjectsInScene;
    private List<SpriteRenderer> _abilityObjectsInView;
    private SpriteRenderer? _selectedAbilityObject = null;
    private Vector2 _previousCameraPosition;
    private Vector2 _boxCastSize = new(4, 4);
    [SerializeField] private float _slowDownFactor = 0.5f;
    [SerializeField] private float _maxAbiltyObjectDistance = 5f;
    private float _originalTimeScale;
    
    private void Start()
    {
        if (Camera.main.IsUnityNull())
            throw new NoMainCameraException();

        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
        _previousCameraPosition = (Vector2)_cameraTransform.position;
        
        Scene currentScene = SceneManager.GetActiveScene();
        _abilityObjectsInScene = GetAbilityObjectsInScene(currentScene).ToList();

        _startScrollingInputActionReference.action.Enable();
        
        _abilityObjectsInView = GetAbilityObjectsInView().ToList();

        _originalTimeScale = Time.timeScale;
    }

    public IAbilityObject? GetSelectedAbilityObject()
    {
        if (_selectedAbilityObject.IsUnityNull() ||
            !_selectedAbilityObject.gameObject.TryGetComponent<IAbilityObject>(out var abilityObjectScript))
            return null;

        return abilityObjectScript;
    }
    
    private void Update()
    {
        
        if (!((Vector2)_cameraTransform.position).Equals(_previousCameraPosition))
            OnCameraChangePosition();

        if (_startScrollingInputActionReference.action.WasPressedThisFrame())
        {
            _playerMovementController.DisablePlayerMovementController();
            Time.timeScale = _slowDownFactor;
        }
        
        if (_startScrollingInputActionReference.action.IsPressed())
        {
            var inputVector = _scrollInputActionReference.action.ReadValue<Vector2>().normalized;

            Vector2 playerPos = transform.position;
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
        if (!_selectedAbilityObject.IsUnityNull() && !IsTransformVisible(_selectedAbilityObject))
        {
            UnHighlightSelected();
            _selectedAbilityObject = null;
        }
    }
    
    private void ScrollThroughAbilityObjects()
    { // Since there may be no ability objects in view, we make sure there are so no exception is thrown
        if (!_abilityObjectsInView.Any())
            return;
            
        if (_selectedAbilityObject.IsUnityNull())
            _selectedAbilityObject = _abilityObjectsInView.First();
        else if (_selectedAbilityObject.Equals(_abilityObjectsInView.Last()))
            _selectedAbilityObject = null;
        else
        {
            int selectedIndex = _abilityObjectsInView.IndexOf(_selectedAbilityObject);

            _selectedAbilityObject = _abilityObjectsInView[++selectedIndex];
        }
    }

    private void UnHighlightSelected()
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
            .Where(renderer => this.IsTransformVisible(renderer.gameObject.transform)); // We check if the renderer is visible
    
    private IEnumerable<SpriteRenderer> GetAbilityObjectsInScene(Scene currentScene) =>
        Resources.FindObjectsOfTypeAll<GameObject>() // We get all GameObjects currently loaded
            .Where(gameObject => gameObject.scene.Equals(currentScene)) // We filter out all GameObjects not in the current scene
            .Where(gameObject => gameObject.TryGetComponent<SpriteRenderer>(out var _)) // We filter out all GameObjects with no SpriteRenderer
            .Where(IAbilityObject.IsAbilityObject) // We filter out all GameObjects that are not an AbilityObject
            .Select(gameObject => gameObject.GetComponent<SpriteRenderer>()); // We select the SpriteRenderer

    private bool IsTransformVisible(Transform abilityObjectTransform)
    {
        Vector2 viewPosition = _mainCamera.WorldToViewportPoint(abilityObjectTransform.position); // returns a vector2 where x and y are 0-1 if the position is in the camera

        return viewPosition.x is >= 0 and <= 1 && viewPosition.y is >= 0 and <= 1;
    }
}