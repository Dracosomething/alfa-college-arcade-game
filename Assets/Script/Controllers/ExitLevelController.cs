using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitLevelController : MonoBehaviour
{
    private const float ExitDelayInSeconds = 2.0f;
    
    [Header("Exit Script Settings")]
    [SerializeField] private InputActionReference _cancelActionReference;
    
    private float _timeRemaining;
    private GameObject _guiIndicator;
    private TextMeshProUGUI _indicatorText;

    private void Awake()
    {
        if (!(SceneHelper.TryFindGameObjectInScene(objectName: "ExitLevelText", out GameObject textObject) &&
              textObject.TryGetComponent<TextMeshProUGUI>(out _indicatorText)))
            throw new MissingComponentException("Could not find component 'TextMeshProUGUI' in GameObject 'ExitLevelText'");
        
        if (!SceneHelper.TryFindGameObjectInScene(objectName: "ExitLevelIndicator", out _guiIndicator))
            throw new MissingComponentException("Could not find GameObject 'ExitLevelIndicator'");
        
        _guiIndicator.SetActive(false);
        _timeRemaining = ExitDelayInSeconds;
        _cancelActionReference.action.Enable();
    }

    private void Update()
    {
        if (_cancelActionReference.action.ReadValue<float>().Equals(1))
        {
            _guiIndicator.SetActive(true);
            
            _timeRemaining -= Time.deltaTime;
            _indicatorText.text = "Exiting level in: " + _timeRemaining.ToString(format: "F1"); 
            // F1 refers to format type for the remaining time to be written in.
            // In specific, one decimal number.
            
            if (_timeRemaining <= 0)
                ExitToMenu();
        }
        
        else if(_timeRemaining < ExitDelayInSeconds)
        {
            _guiIndicator.SetActive(false);
            _timeRemaining = ExitDelayInSeconds;
        }
    }

    private void ExitToMenu() => 
        SceneManager.LoadScene(sceneName: "TitleScreen");
}
