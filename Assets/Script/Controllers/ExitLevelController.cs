using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitLevelController : MonoBehaviour
{
    private const float ExitDelayInSeconds = 2.0f;
    private const string UnchangingIndicatorText = "Exiting level in: ";
    private const string MainMenuSceneName = "TitleScreen";
    
    [Header("Exit Script Settings")]
    [SerializeField] private InputActionReference _cancelActionReference;

    private float _timeRemaining = ExitDelayInSeconds;
    private GameObject _guiIndicator;
    private TextMeshProUGUI _indicatorText;

    private void Awake()
    {
        const string indicatorTextFieldName = "ExitLevelText";
        const string indicatorVisualElementName = "ExitLevelIndicator";
        
        if (!(SceneHelper.TryFindGameObjectInScene(indicatorVisualElementName, out GameObject textObject) &&
              SceneHelper.TryFindGameObjectInScene(indicatorVisualElementName, out _guiIndicator)))
            throw new CouldNotFindGameObjectException(indicatorVisualElementName, indicatorTextFieldName);
        
        if (!textObject.TryGetComponent<TextMeshProUGUI>(out _indicatorText))
            throw new MissingComponentException("Could not find GameObject 'ExitLevelIndicator'");
        
        _guiIndicator.SetActive(false);
        _cancelActionReference.action.Enable();
    }

    private void Update()
    {
        if (_cancelActionReference.action.ReadValueAsInputState() == InputState.Pressed)
        {
            _guiIndicator.SetActive(true);
            
            _timeRemaining -= Time.deltaTime;

            _indicatorText.text = $"{UnchangingIndicatorText}{_timeRemaining.ToString(Constants.SingleDecimalNumericStringFormat)}";

            WhenTimeUpExitToMenu();
        }
        else if(_timeRemaining < ExitDelayInSeconds)
        {
            _guiIndicator.SetActive(false);
            
            _timeRemaining = ExitDelayInSeconds;
        }
    }

    private void WhenTimeUpExitToMenu()
    {
        if (TimeIsUp())
            SceneManager.LoadScene(MainMenuSceneName);
    }

    // If '_timeRemaining' is 0 or less, which means time has run out, then this will return 'True.' Otherwise, it will return 'False.'
    private bool TimeIsUp() =>
        _timeRemaining <= 0; 
}
