using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitLevelController : MonoBehaviour
{
    [Header("Exit Script Behaviour Settings")]
    [SerializeField] private float _exitDelayInSeconds = 2.0f;
    [SerializeField] private InputActionReference _cancelActionReference;

    // [Header("Required Assets")]

    private float _timeRemaining;

    private void Awake()
    {
        _timeRemaining = _exitDelayInSeconds;
        _cancelActionReference.action.Enable();
    }

    private void Update()
    {
        if (_cancelActionReference.action.ReadValue<float>() == 1)
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0)
                ExitToMenu();
        }
        else if(_timeRemaining < 2.0f)
        {
            _timeRemaining = 2.0f;
        }
    }

    private void ExitToMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
